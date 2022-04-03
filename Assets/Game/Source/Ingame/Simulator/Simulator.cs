using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Game.Ingame.Tank;
using Game.Utils;
using UnityEngine;
using NaughtyAttributes;

namespace Game.Ingame.Simulator
{
    public class Simulator : MonoBehaviour
    {
        /// <summary>
        /// Ticks per second (real-time).
        /// </summary>
        [SerializeField] [ReadOnly] int _simulationSpeed = 60;
        [SerializeField] SimulatorSettings _settings;
        [SerializeField] List<TankController> _tankControllers;
        List<Actor> _actors = new();

        List<KeyValuePair<Actor, Vector2>> _bodyPositionInputQueue = new();
        List<KeyValuePair<Actor, float>> _turretRotationInputQueue = new();
        List<Actor> _shootInputQueue = new();

        bool _isConfigured = false;
        bool _isSimulating = false;

        int _simulationTick = 0;
        int _maxSimulationTick = 0;

        Coroutine _runningSimulation;

        public int SimulationSpeed
        {
            get { return _simulationSpeed; }
            set { _simulationSpeed = value; }
        }

        public int SimulationTick => _simulationTick;
        public float TimeStep => 1f / _simulationSpeed;

        void Start()
        {
            if (_settings == null)
            {
                Debug.LogError("Simulator settings not set.");
                return;
            }
            
            foreach (var tankController in _tankControllers)
            {
                tankController.Actor.Speed = _settings.TankSpeed;
                tankController.Actor.TurnSpeed = _settings.TankTurnSpeed;
                tankController.Actor.TurretTurnSpeed = _settings.TankTurretTurnSpeed;
                tankController.Actor.Damage = _settings.TankDamage;
                tankController.Actor.MaxHitPoints = _settings.TankHitpoints;
                tankController.Actor.Radius = _settings.TankRadius;
                tankController.Actor.BulletSpeed = _settings.TankBulletSpeed;

                var initialState = new Actor.State
                {
                    BodyPosition = Actor.UnityToSimulatorPosition(tankController.transform.position),
                    BodyRotation = tankController.transform.rotation.eulerAngles.y,
                    TurretRotation = 0f,
                    
                    HitPoints = tankController.Actor.MaxHitPoints,
                    HasBullet = false
                };
                
                initialState.TargetBodyPosition = initialState.BodyPosition;
                initialState.TargetBodyRotation = initialState.BodyRotation;
                initialState.TargetTurretRotation = initialState.TurretRotation;

                tankController.Actor.History[0] = initialState;
                
                _actors.Add(tankController.Actor);
            }

            Configure();
            Run();
        }

        public void Configure()
        {
            _bodyPositionInputQueue.Clear();
            _turretRotationInputQueue.Clear();
            _shootInputQueue.Clear();
            
            _simulationTick = 0;
            _maxSimulationTick = 0; 

            _isConfigured = true;
        }

        /// <summary>
        /// Start the simulation. Requires a call to <see cref="Configure"/> first.
        /// </summary>
        public void Run()
        {
            if (_isConfigured && !_isSimulating)
            {
                _isSimulating = true;
                _runningSimulation = StartCoroutine(Simulate());
            }
            else
            {
                Debug.LogError("Simulator is not configured or is already running.");
            }
        }
        
        public void Pause()
        {
            _isSimulating = false;
            StopCoroutine(_runningSimulation);
        }
        
        public void Stop()
        {
            Pause();
            _isConfigured = false;
        }

        IEnumerator Simulate()
        {
            while (_isSimulating)
            {
                _simulationTick++;
                _maxSimulationTick = _simulationTick;
                
                // Actual simulation tick happens here
                foreach (var actor in _actors)
                {
                    var actorTransform = actor.GameObject.transform;
                    
                    var previousState = actor.History[_simulationTick - 1];
                    var newState = new Actor.State
                    {
                        BodyRotation = previousState.BodyRotation,
                        TargetBodyRotation = previousState.TargetBodyRotation,
                        
                        BodyPosition = previousState.BodyPosition,
                        TargetBodyPosition = previousState.TargetBodyPosition,
                        
                        TurretRotation = previousState.TurretRotation,
                        TargetTurretRotation = previousState.TargetTurretRotation
                    };

                    actor.History[_simulationTick] = newState;

                    // Body rotation
                    var bodyPositionInputs = _bodyPositionInputQueue.Where(x => x.Key == actor).ToList();
                    foreach (var input in bodyPositionInputs)
                    {
                        newState.TargetBodyPosition = input.Value;
                        var direction = (input.Value - newState.BodyPosition).normalized;
                        var angle = Vector3.SignedAngle(Vector3.forward, Actor.SimulatorToUnityPosition(direction),
                            Vector3.up);
                        newState.TargetBodyRotation = Angle.Normalize(angle);
                        _bodyPositionInputQueue.Remove(input);
                    }
                    newState.BodyRotation = Mathf.MoveTowardsAngle(previousState.BodyRotation,
                        newState.TargetBodyRotation, actor.TurnSpeed * TimeStep);

                    // Body position
                    if (IsAtRotation(actor, newState.TargetBodyRotation, _simulationTick))
                    {
                        newState.BodyPosition = Vector2.MoveTowards(newState.BodyPosition,
                            newState.TargetBodyPosition, actor.Speed * TimeStep);
                    }

                    // Turret rotation
                    var turretRotationInputs = _turretRotationInputQueue.Where(x => x.Key == actor).ToList();
                    foreach (var input in turretRotationInputs)
                    {
                        newState.TargetTurretRotation = input.Value;
                        _turretRotationInputQueue.Remove(input);
                    }
                    newState.TurretRotation = Mathf.MoveTowardsAngle(previousState.TurretRotation,
                        newState.TargetTurretRotation, actor.TurretTurnSpeed * TimeStep);
                }
                
                yield return new WaitForSecondsRealtime(TimeStep);
            }

            yield return null;
        }

        public void AddActor(Actor actor)
        {
            _actors.Add(actor);
        }

        public void RemoveActor(Actor actor)
        {
            _actors.Remove(actor);
        }
        
        public bool IsAtPosition(Actor actor, Vector2 position, int tick)
        {
            if (tick < 0 || tick > _maxSimulationTick)
            {
                Debug.LogError("Position check: requested tick out of range.");
                return false;
            }
            
            var bodyPosition = actor.History[tick].BodyPosition;
            return Vector2.Distance(bodyPosition, position) < actor.Radius;
        }
        
        public bool IsAtRotation(Actor actor, float rotation, int tick)
        {
            if (tick < 0 || tick > _maxSimulationTick)
            {
                Debug.LogError("Rotation check: requested tick out of range.");
                return false;
            }
            
            rotation %= 360;
            var bodyRotation = actor.History[tick].BodyRotation % 360f;
            return Mathf.Abs(bodyRotation - rotation) < 1f;
        }

        public void EnqueueBodyPositionInput(Actor actor, Vector2 targetPosition)
        {
            _bodyPositionInputQueue.Add(new KeyValuePair<Actor, Vector2>(actor, targetPosition));
        }

        public void EnqueueTurretRotationInput(Actor actor, float targetRotation)
        {
            _turretRotationInputQueue.Add(new KeyValuePair<Actor, float>(actor, targetRotation));
        }
        
        public void EnqueueShootInput(Actor actor)
        {
            _shootInputQueue.Add(actor);
        }
    }
}
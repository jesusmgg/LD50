using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

        List<Actor> _actors = new();

        List<KeyValuePair<Actor, Vector2>> _bodyPositionInputQueue = new();
        List<KeyValuePair<Actor, float>> _bodyRotationInputQueue = new();
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

        public int TimeStep => 1 / _simulationSpeed;

        public void Configure(List<Actor> actors)
        {
            _actors = actors;

            _bodyPositionInputQueue.Clear();
            _bodyRotationInputQueue.Clear();
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
                
                // Actual simulation tick happens here
                foreach (var actor in _actors)
                {
                    var previousState = actor.History[_simulationTick - 1];
                    var newState = new Actor.State();
                    
                    // Body rotation
                    newState.TargetBodyRotation = previousState.TargetBodyRotation;
                    newState.BodyRotation = previousState.BodyRotation;
                    foreach (var input in _bodyRotationInputQueue.Where(x => x.Key == actor))
                    {
                        newState.TargetBodyRotation = input.Value;
                        _bodyRotationInputQueue.Remove(input);
                    }
                    newState.BodyRotation = Mathf.MoveTowardsAngle(previousState.BodyRotation,
                        newState.TargetBodyRotation, actor.TurnSpeed * TimeStep);
                    
                    // Body position
                    newState.TargetBodyPosition = previousState.TargetBodyPosition;
                    newState.BodyPosition = previousState.BodyPosition;
                    foreach (var input in _bodyPositionInputQueue.Where(x => x.Key == actor))
                    {
                        newState.TargetBodyPosition = input.Value;
                        _bodyPositionInputQueue.Remove(input);
                    }
                    if (IsAtRotation(actor, newState.TargetBodyRotation, _simulationTick))
                    {
                        newState.BodyPosition = Vector2.MoveTowards(newState.BodyPosition,
                            newState.TargetBodyPosition, actor.Speed * TimeStep);
                    }
                    
                    // Turret rotation
                    newState.TargetTurretRotation = previousState.TargetTurretRotation;
                    newState.TurretRotation = previousState.TurretRotation;
                    foreach (var input in _turretRotationInputQueue.Where(x => x.Key == actor))
                    {
                        newState.TargetTurretRotation = input.Value;
                        _turretRotationInputQueue.Remove(input);
                    }
                    newState.TurretRotation = Mathf.MoveTowardsAngle(previousState.TurretRotation,
                        newState.TargetTurretRotation, actor.TurnSpeed * TimeStep);
                    
                    actor.History[_simulationTick] = newState;
                }

                _maxSimulationTick = _simulationTick;
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

        public void EnqueueBodyRotationInput(Actor actor, float targetRotation)
        {
            _bodyRotationInputQueue.Add(new KeyValuePair<Actor, float>(actor, targetRotation));
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
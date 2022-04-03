using System.Collections.Generic;
using Cinemachine;
using Game.Ingame.Simulator;
using Game.Utils;
using UnityEngine;
using Zenject;

namespace Game.Ingame.Tank
{
    public class TankController : MonoBehaviour
    {
        [SerializeField] private Transform _turretTransform;

        List<MeshRenderer> _meshRenderers;
        Simulator.Simulator _simulator;
        CinemachineVirtualCameraBase _virtualCamera;
        CinemachineBrain _cinemachineBrain;

        public Actor Actor { get; private set; }
        // public bool IsMoving => _currentMovementInput.magnitude > 0.01f;

        [Inject]
        void Construct(
            List<MeshRenderer> meshRenderers,
            Simulator.Simulator simulator,
            CinemachineVirtualCameraBase virtualCamera,
            CinemachineBrain cinemachineBrain
        )
        {
            _meshRenderers = meshRenderers;
            _simulator = simulator;
            _virtualCamera = virtualCamera;
            _cinemachineBrain = cinemachineBrain;
        }

        void Awake()
        {
            Actor = new Actor();
            Actor.GameObject = gameObject;
        }

        void Update()
        {
            var isAlive = IsAlive();
            foreach (var meshRenderer in _meshRenderers)
            {
                meshRenderer.enabled = isAlive;
            }

            if (isAlive)
            {
                var state = Actor.History[_simulator.SimulationTick];
                var simulatorPosition = state.BodyPosition;
                var simulatorRotation = state.BodyRotation;
                var simulatorTurretRotation = state.TurretRotation;

                var tankTransform = transform;
                var turretRotation = _turretTransform.eulerAngles;
                tankTransform.position =
                    new Vector3(simulatorPosition.x, tankTransform.position.y, simulatorPosition.y);
                tankTransform.rotation = Quaternion.Euler(0, simulatorRotation, 0);
                _turretTransform.rotation =
                    Quaternion.Euler(turretRotation.x, simulatorTurretRotation, turretRotation.z);
            }
        }

        public bool IsAlive()
        {
            return _simulator.IsAlive(Actor, _simulator.SimulationTick);
        }
        
        public bool IsDamaged()
        {
            return Actor.History[_simulator.SimulationTick].HitPoints < Actor.MaxHitPoints;
        }

        public void InputTargetPosition(Vector3 targetPosition)
        {
            if (IsAlive())
            {
                var position = Actor.UnityToSimulatorPosition(targetPosition);
                _simulator.EnqueueBodyPositionInput(Actor, position);
            }
        }

        public void InputTurretTargetPosition(Vector3 targetPosition)
        {
            if (IsAlive())
            {
                var direction = targetPosition - _turretTransform.position;
                var angle = Vector3.SignedAngle(Vector3.forward, direction, Vector3.up);
                _simulator.EnqueueTurretRotationInput(Actor, Angle.Normalize(angle));
            }
        }

        public void InputShoot()
        {
            if (IsAlive())
            {
                _simulator.EnqueueShootInput(Actor);
            }
        }
    }
}
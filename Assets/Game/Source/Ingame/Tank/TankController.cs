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

        Simulator.Simulator _simulator;
        CinemachineVirtualCameraBase _virtualCamera;
        CinemachineBrain _cinemachineBrain;

        public Actor Actor { get; private set; }
        // public bool IsMoving => _currentMovementInput.magnitude > 0.01f;

        [Inject]
        void Construct(
            Simulator.Simulator simulator,
            CinemachineVirtualCameraBase virtualCamera,
            CinemachineBrain cinemachineBrain
        )
        {
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
            var simulatorPosition = Actor.History[_simulator.SimulationTick].BodyPosition;
            var simulatorRotation = Actor.History[_simulator.SimulationTick].BodyRotation;
            var simulatorTurretRotation = Actor.History[_simulator.SimulationTick].TurretRotation;

            var tankTransform = transform;
            var turretRotation  = _turretTransform.eulerAngles;
            tankTransform.position = new Vector3(simulatorPosition.x, tankTransform.position.y, simulatorPosition.y);
            tankTransform.rotation = Quaternion.Euler(0, simulatorRotation, 0);
            _turretTransform.rotation = Quaternion.Euler(turretRotation.x, simulatorTurretRotation, turretRotation.z);
        }

        public void InputTargetPosition(Vector3 targetPosition)
        {
            var position = Actor.UnityToSimulatorPosition(targetPosition);
            _simulator.EnqueueBodyPositionInput(Actor, position);
        }

        public void InputTurretTargetPosition(Vector3 targetPosition)
        {
            var direction = targetPosition - _turretTransform.position;
            var angle = Vector3.SignedAngle(Vector3.forward, direction, Vector3.up);
            _simulator.EnqueueTurretRotationInput(Actor, Angle.Normalize(angle));
        }
    }
}
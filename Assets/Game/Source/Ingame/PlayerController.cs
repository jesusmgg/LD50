using Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

namespace Game.Ingame
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] float _speed = 5f;
        [SerializeField] float _turnSpeed = 360f;

        [SerializeField] Rigidbody _rigidbody;
        [SerializeField] Transform _playerTransform;
        
        Vector3 _currentMovementInput;
        float _movementSkewAngle;

        PlayerInput _input;
        CinemachineVirtualCameraBase _mainVCam;

        public bool IsMoving => _currentMovementInput.magnitude > 0.01f;
        public Vector3 Velocity => _currentMovementInput * _speed;
        public Vector3 VelocityNormalized => _currentMovementInput;

        [Inject]
        void Construct(
            PlayerInput input,
            CinemachineVirtualCameraBase mainVCam
        )
        {
            _input = input;
            _mainVCam = mainVCam;
        }

        void Start()
        {
            _movementSkewAngle = _mainVCam.transform.eulerAngles.y;
        }

        void OnEnable()
        {
            _input.onActionTriggered += OnInputActionTriggered;
        }

        void OnDisable()
        {
            _input.onActionTriggered -= OnInputActionTriggered;
        }

        void Update()
        {
            UpdateRotation();
        }

        void FixedUpdate()
        {
            UpdatePosition();
        }

        void UpdateRotation()
        {
            if (_currentMovementInput != Vector3.zero)
            {
                var matrix = Matrix4x4.Rotate(Quaternion.Euler(0, _mainVCam.transform.eulerAngles.y, 0));
                var skewedInput = matrix.MultiplyPoint3x4(_currentMovementInput);

                var tr = _playerTransform;
                var position = tr.position;
                var relative = position + skewedInput - position;
                var rotation = Quaternion.LookRotation(relative, Vector3.up);

                _playerTransform.rotation =
                    Quaternion.RotateTowards(tr.rotation, rotation, _turnSpeed * Time.deltaTime);
            }
        }

        void UpdatePosition()
        {
            var tr = _playerTransform;
            _rigidbody.MovePosition(
                tr.position + tr.forward * _currentMovementInput.magnitude * _speed * Time.deltaTime);
        }

        void OnInputActionTriggered(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                switch (context.action.name)
                {
                    case "Movement":
                        HandleMovement(context);
                        break;
                }
            }
        }

        void HandleMovement(InputAction.CallbackContext context)
        {
            Vector2 movement = context.action.ReadValue<Vector2>();
            _currentMovementInput = new Vector3(movement.x, 0, movement.y);
        }
    }
}
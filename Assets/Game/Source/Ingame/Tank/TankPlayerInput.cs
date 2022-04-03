using UnityEngine;
using Zenject;

namespace Game.Ingame.Tank
{
    public class TankPlayerInput : MonoBehaviour
    {
        [SerializeField] LayerMask _mouseRaycastMask;

        Camera _mainCamera;
        TankController _tankController;

        public TankController TankController => _tankController;

        [Inject]
        void Construct(TankController tankController)
        {
            _tankController = tankController;
        }

        void Start()
        {
            _mainCamera = Camera.main;
        }

        void Update()
        {
            bool isOverUI = UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject();
            if (Input.GetMouseButtonDown(0) && !isOverUI)
            {
                var point = GetMousePositionOnGround();
                if (point.HasValue)
                {
                    _tankController.InputTurretTargetPosition(point.Value);
                }
            }

            if (Input.GetMouseButtonDown(1) && !isOverUI)
            {
                var point = GetMousePositionOnGround();
                if (point.HasValue)
                {
                    _tankController.InputTargetPosition(point.Value);
                }
            }
            
            if (Input.GetKeyDown(KeyCode.Space))
            {
                InputShoot();
            }
        }

        Vector3? GetMousePositionOnGround()
        {
            Ray ray = _mainCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, _mouseRaycastMask))
            {
                return hit.point;
            }
            else
            {
                return null;
            }
        }

        public void InputShoot()
        {
            _tankController.InputShoot();
        }
    }
}
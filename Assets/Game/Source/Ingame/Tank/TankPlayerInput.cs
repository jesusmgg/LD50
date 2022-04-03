using UnityEngine;
using Zenject;

namespace Game.Ingame.Tank
{
    public class TankPlayerInput : MonoBehaviour
    {
        [SerializeField] LayerMask _mouseRaycastMask;

        Camera _mainCamera;
        TankController _tankController;

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
                _tankController.InputTurretTargetPosition(point);
            }

            if (Input.GetMouseButtonDown(1) && !isOverUI)
            {
                var point = GetMousePositionOnGround();
                _tankController.InputTargetPosition(point);
            }
        }

        Vector3 GetMousePositionOnGround()
        {
            Ray ray = _mainCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, _mouseRaycastMask))
            {
                return hit.point;
            }
            else
            {
                return Vector3.zero;
            }
        }

        public void InputShoot()
        {
            _tankController.InputShoot();
        }
    }
}
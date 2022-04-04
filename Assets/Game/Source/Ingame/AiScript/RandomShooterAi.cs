using System;
using Cysharp.Threading.Tasks;
using Game.Ingame.Tank;
using UnityEngine;
using Zenject;
using Random = UnityEngine.Random;

namespace Game.Ingame.AiScript
{
    public class RandomShooterAi : MonoBehaviour
    {
        [SerializeField] bool _canMove;
        [SerializeField] bool _canAim;
        [SerializeField] bool _canShoot;
        
        [SerializeField] float _roamingRange = 4f;
        
        [SerializeField] Vector2 _moveInterval = new(.5f, 2f);
        [SerializeField] Vector2 _aimInterval = new(.5f, 2f);
        [SerializeField] Vector2 _shootInterval = new(.5f, 2f);
        
        [SerializeField] LayerMask _obstacleLayerMask;
        
        bool _isSimulating;

        TankController _playerTankController;
        TankPlayerInput _playerInput;
        TankController _tankController;
        Simulator.Simulator _simulator;

        [Inject]
        public void Construct(TankController tankController, TankPlayerInput tankPlayerInput, Simulator.Simulator simulator)
        {
            _tankController = tankController;
            _playerInput = tankPlayerInput;
            _simulator = simulator;
        }

        void Start()
        {
            _playerTankController = _playerInput.TankController;
            _isSimulating = true;

            UpdatePosition().Forget();
            UpdateTurret().Forget();
            Shoot().Forget();
        }

        async UniTask UpdatePosition()
        {
            while (_isSimulating)
            {
                if (_canMove && _simulator.IsSimulating && _tankController.IsAlive())
                {
                    var position = transform.position;
                    Vector3 targetPosition;
                    Vector3 direction;
                    float distance;
                    do
                    {
                        Vector3 translation = new Vector3(1f, 0, 1f) * Random.Range(1f, _roamingRange);
                        translation = Quaternion.AngleAxis(Random.Range(0f, 359f), Vector3.up) * translation;

                        targetPosition = position + translation;
                        direction = targetPosition - position;
                        distance = direction.magnitude;
                    } while (Physics.Raycast(position, direction, out RaycastHit _, distance, _obstacleLayerMask));

                    _tankController.InputTargetPosition(targetPosition);
                }

                await UniTask.Delay(TimeSpan.FromSeconds(Random.Range(_moveInterval.x, _moveInterval.y)));
            }
        }

        async UniTask UpdateTurret()
        {
            while (_isSimulating)
            {
                if (_canAim && _simulator.IsSimulating && _tankController.IsAlive())
                {
                    _tankController.InputTurretTargetPosition(_playerTankController.transform.position);
                }

                await UniTask.Delay(TimeSpan.FromSeconds(Random.Range(_aimInterval.x, _aimInterval.y)));
            }
        }

        async UniTask Shoot()
        {
            while (_isSimulating)
            {
                if (_canShoot && _simulator.IsSimulating && _tankController.IsAlive())
                {
                    _tankController.InputShoot();
                }

                await UniTask.Delay(TimeSpan.FromSeconds(Random.Range(_shootInterval.x, _shootInterval.y)));
            }
        }
    }
}
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
        [SerializeField] float _roamingRange = 4f;
        
        bool _isSimulating;

        TankController _playerTankController;
        TankPlayerInput _playerInput;
        TankController _tankController;

        [Inject]
        public void Construct(TankController tankController, TankPlayerInput tankPlayerInput)
        {
            _tankController = tankController;
            _playerInput = tankPlayerInput;
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
                if (_tankController.IsAlive())
                {
                    var translation = new Vector3(1f, 0, 1f) * Random.Range(1f, _roamingRange);
                    translation = Quaternion.AngleAxis(Random.Range(0f, 359f), Vector3.up) * translation;
                    _tankController.InputTargetPosition(transform.position + translation);
                }

                await UniTask.Delay(TimeSpan.FromSeconds(Random.Range(0.5f, 2f)));
            }
        }

        async UniTask UpdateTurret()
        {
            while (_isSimulating)
            {
                if (_tankController.IsAlive())
                {
                    _tankController.InputTurretTargetPosition(_playerTankController.transform.position);
                }

                await UniTask.Delay(TimeSpan.FromSeconds(Random.Range(0.5f, 2f)));
            }
        }

        async UniTask Shoot()
        {
            while (_isSimulating)
            {
                if (_tankController.IsAlive())
                {
                    _tankController.InputShoot();
                }

                await UniTask.Delay(TimeSpan.FromSeconds(Random.Range(0.5f, 2f)));
            }
        }
    }
}
using Game.Ingame.Simulator;
using Game.Ingame.Tank;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Game.Ui
{
    [RequireComponent(typeof(Button))]
    public class ShootButton : MonoBehaviour
    {
        Button _button;
        
        TankPlayerInput _playerInput;
        
        [Inject]
        public void Construct(TankPlayerInput playerInput)
        {
            _playerInput = playerInput;
        }

        void Awake()
        {
            _button = GetComponent<Button>();
        }

        void OnEnable()
        {
            _button.onClick.AddListener(Shoot);
        }
        
        void OnDisable()
        {
            _button.onClick.RemoveListener(Shoot);
        }

        void Shoot()
        {
            _playerInput.InputShoot();
        }
    }
}
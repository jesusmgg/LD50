using UnityEngine;
using Zenject;

namespace Game.Ingame.Tank
{
    public class TankDamageEffects : MonoBehaviour
    {
        [SerializeField] ParticleSystem _smoke;
        
        bool _isSmokePlaying;
        
        TankController _tankController;
        
        [Inject]
        void Initialize(TankController tankController)
        {
            _tankController = tankController;
        }

        void Update()
        {
            var isAlive = _tankController.IsAlive();
            var isDamaged = _tankController.IsDamaged();
            
            if (isAlive && isDamaged)
            {
                if (!_isSmokePlaying)
                {
                    _smoke.Play();
                    _isSmokePlaying = true;
                }
            }
            else
            {
                if (_isSmokePlaying)
                {
                    _smoke.Stop();
                    _isSmokePlaying = false;
                }
            }
        }
    }
}
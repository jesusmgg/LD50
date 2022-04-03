using UnityEngine;
using Zenject;

namespace Game.Ingame
{
    public class BulletParticleManager : MonoBehaviour
    {
        [SerializeField] ParticleSystem _travelParticleSystem;
        [SerializeField] ParticleSystem _hitParticleSystem;
        
        [SerializeField] int _travelParticleEmitCount = 2;
        [SerializeField] int _hitParticleEmitCount = 30;
        
        [SerializeField] float _particleHeight = 1.1f;

        Simulator.Simulator _simulator;
        
        [Inject]
        public void Construct(Simulator.Simulator simulator)
        {
            _simulator = simulator;
        }

        void OnEnable()
        {
            _simulator.OnBulletTravel.AddListener(EmitTravelParticle);
            _simulator.OnBulletHit.AddListener(EmitHitParticle);
        }
        
        void OnDisable()
        {
            _simulator.OnBulletTravel.RemoveListener(EmitTravelParticle);
            _simulator.OnBulletHit.RemoveListener(EmitHitParticle);
        }

        public void EmitTravelParticle(Vector3 position)
        {
            position.y = _particleHeight;
            _travelParticleSystem.transform.position = position;
            _travelParticleSystem.Emit(_travelParticleEmitCount);
        }
        
        public void EmitHitParticle(Vector3 position)
        {
            position.y = _particleHeight;
            _hitParticleSystem.transform.position = position;
            _hitParticleSystem.Emit(_hitParticleEmitCount);
        }
    }
}
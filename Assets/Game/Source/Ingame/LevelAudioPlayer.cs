using UnityEngine;
using Zenject;
using Random = UnityEngine.Random;

namespace Game.Ingame
{
    public class LevelAudioPlayer : MonoBehaviour
    {
        [SerializeField] AudioClip[] _shootClips;
        [SerializeField] AudioClip[] _hitClips;

        [SerializeField] AudioSource _audioSource;
        
        Simulator.Simulator _simulator;

        [Inject]
        void Initialize(Simulator.Simulator simulator)
        {
            _simulator = simulator;
        }

        void OnEnable()
        {
            _simulator.OnBulletShoot.AddListener(OnBulletShoot);
            _simulator.OnBulletHit.AddListener(OnBulletHit);
        }
        
        void OnDisable()
        {
            _simulator.OnBulletShoot.RemoveListener(OnBulletShoot);
            _simulator.OnBulletHit.RemoveListener(OnBulletHit);
        }

        public void PlayAtPosition(AudioClip[] clips, Vector3 position)
        {
            if (clips.Length > 0)
            {
                int index = Random.Range(0, clips.Length);
                transform.position = position;
                _audioSource.PlayOneShot(clips[index]);
                // AudioSource.PlayClipAtPoint(clips[index], position);
            }
        }

        void OnBulletShoot(Vector3 position)
        {
            PlayAtPosition(_shootClips, position);
        }

        void OnBulletHit(Vector3 position)
        {
            PlayAtPosition(_hitClips, position);
        }
    }
}
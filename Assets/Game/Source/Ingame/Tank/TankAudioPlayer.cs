using UnityEngine;
using Zenject;

namespace Game.Ingame.Tank
{
    public class TankAudioPlayer : MonoBehaviour
    {
        [SerializeField] AudioClip _clip;
        [SerializeField] AudioSource _audioSource;

        [SerializeField] Vector2 _enginePitchRange = new(1f, 1.5f);

        TankController _tankController;
        Simulator.Simulator _simulator;

        [Inject]
        void Initialize(TankController tankController, Simulator.Simulator simulator)
        {
            _tankController = tankController;
            _simulator = simulator;
        }

        void Start()
        {
            _audioSource.clip = _clip;
        }

        void Update()
        {
            _audioSource.pitch = _tankController.IsMoving ? _enginePitchRange.y : _enginePitchRange.x;

            if (_audioSource.isPlaying)
            {
                if (!_tankController.IsAlive())
                {
                    _audioSource.Stop();
                }
            }
            else
            {
                if (_tankController.IsAlive())
                {
                    _audioSource.Play();
                }
            }
        }
    }
}
using UnityEngine;
using Zenject;

namespace Game.Ingame
{
    public class PlayerAnimationController : MonoBehaviour
    {
        [SerializeField] Animator _animator;
        [SerializeField] PlayerController _playerController;
        
        static readonly int IsMoving = Animator.StringToHash("IsMoving");
        static readonly int Speed = Animator.StringToHash("Speed");

        [Inject]
        void Construct() { }

        void Update()
        {
            _animator.SetBool(IsMoving, _playerController.IsMoving);
            _animator.SetFloat(Speed, _playerController.VelocityNormalized.magnitude);
        }
    }
}
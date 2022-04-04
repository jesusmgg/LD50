using UnityEngine;
using Zenject;

namespace Game.Ingame.Tank
{
    public class TankAnimationController : MonoBehaviour
    {
        [SerializeField] Animator _animator;
        [SerializeField] TankController _tankController;
        
        static readonly int IsMoving = Animator.StringToHash("IsMoving");

        [Inject]
        void Construct() { }

        void Update()
        {
            _animator.SetBool(IsMoving, _tankController.IsMoving);
        }
    }
}
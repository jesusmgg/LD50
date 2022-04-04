using UnityEngine;

namespace Game.Ui
{
    public class LevelInitialDialog : MonoBehaviour
    {
        [SerializeField] Animator _animator;
        static readonly int IsVisible = Animator.StringToHash("IsVisible");

        void Start()
        {
            _animator.SetBool(IsVisible, true);
        }

        public void Hide()
        {
            _animator.SetBool(IsVisible, false);
        }
    }
}
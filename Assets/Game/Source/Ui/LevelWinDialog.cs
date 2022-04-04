using System;
using Game.Ingame;
using UnityEngine;
using Zenject;

namespace Game.Ui
{
    public class LevelWinDialog : MonoBehaviour
    {
        [SerializeField] Animator _animator;

        LevelScript _levelScript;

        static readonly int IsVisible = Animator.StringToHash("IsVisible");

        [Inject]
        public void Construct(LevelScript levelScript)
        {
            _levelScript = levelScript;
        }

        void OnEnable()
        {
            _levelScript.OnWin.AddListener(OnLevelWin);
        }
        
        void OnDisable()
        {
            _levelScript.OnWin.RemoveListener(OnLevelWin);
        }

        void OnLevelWin()
        {
            _animator.SetBool(IsVisible, true);
        }
    }
}
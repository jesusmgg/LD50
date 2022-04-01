using Game.GameSystems.Scenes;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Game.Ui
{
    /// <summary>
    /// Generic component to attach to a button to transition to a scene <see cref="SceneName"/> on click.
    /// </summary>
    [RequireComponent(typeof(Button))]
    public class LoadSceneButton : MonoBehaviour
    {
        public string SceneName;
        
        Button _button;
        
        SceneManager _sceneManager;
        
        [Inject]
        public void Construct(SceneManager sceneManager)
        {
            _sceneManager = sceneManager;
        }
        
        void Awake()
        {
            _button = GetComponent<Button>();
        }

        void OnEnable()
        {
            _button.onClick.AddListener(LoadScene);
        }
        
        void OnDisable()
        {
            _button.onClick.RemoveListener(LoadScene);
        }

        public void LoadScene()
        {
            _sceneManager.TransitionToScene(SceneName);
        }
    }
}
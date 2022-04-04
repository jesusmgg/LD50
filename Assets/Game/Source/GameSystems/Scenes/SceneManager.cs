using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using Zenject;

namespace Game.GameSystems.Scenes
{
    public class SceneManager : MonoBehaviour, IGameSystem
    {
        [SerializeField] string startingScene;

        Scene _currentScene;

        public bool IsLoadingScene { get; private set; }

        public readonly SceneManagerSceneNameEvent onSceneLoadStart = new SceneManagerSceneNameEvent();
        public readonly SceneManagerSceneEvent onSceneLoadDone = new SceneManagerSceneEvent();
        public readonly SceneManagerSceneEvent onSceneUnloadStart = new SceneManagerSceneEvent();
        public readonly SceneManagerSceneEvent onSceneUnloadDone = new SceneManagerSceneEvent();

        [Inject]
        void Construct() { }

        public void Setup() { }

        IEnumerator Start()
        {
            // Unload all but the first
            int c = UnityEngine.SceneManagement.SceneManager.sceneCount;
            for (int i = 0; i < c; i++)
            {
                Scene scene = UnityEngine.SceneManagement.SceneManager.GetSceneAt(i);
                if (scene.name != "MainScene")
                {
                    UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync(scene);
                }
            }

            // Load starting scene
            if (!string.IsNullOrWhiteSpace(startingScene))
            {
                TransitionToScene(startingScene);
            }

            yield return null;
        }

        public void TransitionToScene(string sceneName, bool setActive = true)
        {
            StartCoroutine(DoLoadScene(sceneName, setActive));
        }

        IEnumerator DoLoadScene(string sceneName, bool setActive)
        {
            // TODO: show loader UI (spinner or something)

            if (IsLoadingScene)
            {
                Debug.LogWarning($"SceneManager: won't load {sceneName}, already loading another scene.");
                yield return null;
            }

            else
            {
                IsLoadingScene = true;

                if (_currentScene.IsValid())
                {
                    onSceneUnloadStart.Invoke(_currentScene);
                    var unloadTask = UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync(_currentScene);
                    yield return new WaitUntil(() => unloadTask.isDone);
                    onSceneUnloadDone.Invoke(_currentScene);
                }

                onSceneLoadStart.Invoke(sceneName);
                var loadTask =
                    UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
                yield return new WaitUntil(() => loadTask.isDone);
                _currentScene = UnityEngine.SceneManagement.SceneManager.GetSceneByName(sceneName);
                onSceneLoadDone.Invoke(_currentScene);

                if (setActive)
                {
                    UnityEngine.SceneManagement.SceneManager.SetActiveScene(_currentScene);
                }

                IsLoadingScene = false;
            }
        }

        public class SceneManagerSceneEvent : UnityEvent<Scene> { }

        public class SceneManagerSceneNameEvent : UnityEvent<string> { }
    }
}
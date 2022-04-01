using System.Collections.Generic;
using Game.GameSystems.Scenes;
using UnityEngine;
using Zenject;

namespace Game.GameSystems
{
    /// <summary>
    /// Initializes all game systems in the correct order. This manager should be present in the game main scene and
    /// only once.
    /// <seealso cref="IGameSystem"/>
    /// </summary>
    public class GameSystemManager : MonoBehaviour
    {
        /// <summary>
        /// Game systems (components implementing <see cref="IGameSystem"/>) will be set up in the order specified by
        /// this list. The game systems should be injected with Zenject and then added to the list.
        /// </summary>
        List<IGameSystem> _gameSystems;
        
        // Game systems (injected)
        SceneManager _sceneManager;

        [Inject]
        void Construct(SceneManager sceneManager)
        {
            _sceneManager = sceneManager;
        }

        void Awake()
        {
            _gameSystems = new List<IGameSystem>
            {
                _sceneManager
            };

            foreach (IGameSystem gameSystem in _gameSystems)
            {
                gameSystem.Setup();
            }
        }
    }
}
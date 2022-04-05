using Game.Ingame;
using TMPro;
using UnityEngine;
using Zenject;

namespace Game.Ui
{
    public class LevelEnemyCounter : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI _text;
        
        LevelScript _levelScript;
        
        [Inject]
        void Construct(LevelScript levelScript)
        {
            _levelScript = levelScript;
        }

        void Update()
        {
            _text.text = $"Enemies: {_levelScript.EnemyCount.ToString()}";
        }
    }
}
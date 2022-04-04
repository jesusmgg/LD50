using System.Collections.Generic;
using System.Linq;
using Game.Ingame.Tank;
using UnityEngine;
using UnityEngine.Events;
using Zenject;

namespace Game.Ingame
{
    public class LevelScript : MonoBehaviour
    {
        List<TankController> _tankControllers;
        
        Simulator.Simulator _simulator;
        
        public bool IsWon {get; private set;}
        public int EnemyCount {get; private set;}
        
        public UnityEvent OnWin = new();

        [Inject]
        void Construct(Simulator.Simulator simulator)
        {
            _simulator = simulator;
        }

        void Start()
        {
            _tankControllers = _simulator.TankControllers;
        }

        void Update()
        {
            EnemyCount = _tankControllers.Count(x => x.IsAlive() && !x.IsPlayer);
            if (_simulator.IsSimulating && _simulator.SimulationTick > 0 && !IsWon && EnemyCount <= 0)
            {
                IsWon = true;
                OnWin.Invoke();
            }
        }
    }
}
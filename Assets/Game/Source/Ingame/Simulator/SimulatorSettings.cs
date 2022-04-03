using UnityEngine;

namespace Game.Ingame.Simulator
{
    [CreateAssetMenu(fileName = "SimulatorSettings", menuName = "Settings/SimulatorSettings", order = 0)]
    public class SimulatorSettings : ScriptableObject
    {
        public float TankSpeed;
        public float TankTurnSpeed;
        public int TankHitpoints;
        public int TankDamage;
        public float TankBulletSpeed;
        public float TankTurretTurnSpeed;
        public float TankRadius;
    }
}
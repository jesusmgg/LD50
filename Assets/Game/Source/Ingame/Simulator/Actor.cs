using UnityEngine;

namespace Game.Ingame.Simulator
{
    public class Actor
    {
        /// <summary>
        /// Ticks allocated for history: minutes * seconds * framesPerSecond.
        /// Assuming 1 hour of gameplay. The game will probably crash if this is not enough.
        /// </summary>
        const int TickAllocation = 60 * 60 * 60;
        
        public float Speed { get; set; }
        public float TurnSpeed { get; set; }
        public float TurretTurnSpeed { get; set; }
        public float TurretLength { get; set; }
        public int MaxHitPoints { get; set; }
        public int Damage { get; set; }
        public float BulletSpeed { get; set; }
        public float BulletRange { get; set; }
        public float Radius { get; set; }
        
        public GameObject GameObject { get; set; }
        
        /// <summary>
        /// Index is used to keep track of the current simulation tick.
        /// </summary>
        public State[] History { get; set; }

        public Actor()
        {
            History = new State[TickAllocation];
        }
        
        public static Vector2 UnityToSimulatorPosition(Vector3 position)
        {
            return new Vector2(position.x, position.z);
        }
        
        public static Vector3 SimulatorToUnityPosition(Vector2 position)
        {
            return new Vector3(position.x, 0, position.y);
        }
        
        public class State
        {
            public Vector2 BodyPosition { get; set; }
            public float BodyRotation { get; set; }
            public float TurretRotation { get; set; }
            
            public Vector2 TargetBodyPosition { get; set; }
            public float TargetBodyRotation { get; set; }
            public float TargetTurretRotation { get; set; }
            
            public bool HasBullet { get; set; }
            public Vector2 BulletDirection { get; set; }
            public Vector2 BulletPosition { get; set; }
            
            public int HitPoints { get; set; }
        }
    }
}
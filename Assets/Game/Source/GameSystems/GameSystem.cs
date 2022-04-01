namespace Game.GameSystems
{
    /// <summary>
    /// All the game main systems (for example scene management, data management, and such) should implement this
    /// interface and be added to the <see cref="GameSystemManager"/> (via Zenject) for proper initialization.
    /// </summary>
    public interface IGameSystem
    {
        public void Setup();
    }
}
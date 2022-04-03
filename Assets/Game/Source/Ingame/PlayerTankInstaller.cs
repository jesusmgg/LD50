using Game.Ingame.Tank;
using Zenject;

namespace Game.Ingame
{
    public class PlayerTankInstaller : MonoInstaller<PlayerTankInstaller>
    {
        public override void InstallBindings()
        {
            Container.Bind<TankPlayerInput>().FromComponentInHierarchy().AsSingle();
        }
    }
}
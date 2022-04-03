using Zenject;

namespace Game.Ingame
{
    public class SimulatorInstaller : MonoInstaller<SimulatorInstaller>
    {
        public override void InstallBindings()
        {
            Container.Bind<Simulator.Simulator>().FromComponentInHierarchy().AsSingle();
        }
    }
}
using Zenject;

namespace Game.Ingame
{
    public class LevelScriptInstaller : MonoInstaller<LevelScriptInstaller>
    {
        public override void InstallBindings()
        {
            Container.Bind<LevelScript>().FromComponentInHierarchy().AsSingle();
        }
    }
}
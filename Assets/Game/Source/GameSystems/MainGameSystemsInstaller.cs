using Game.GameSystems.Scenes;
using Zenject;

namespace Game.GameSystems
{
    public class MainGameSystemsInstaller : MonoInstaller<MainGameSystemsInstaller>
    {
        public override void InstallBindings()
        {
            Container.Bind<SceneManager>().FromComponentInHierarchy().AsSingle().NonLazy();
        }
    }
}
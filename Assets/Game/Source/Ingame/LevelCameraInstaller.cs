using Cinemachine;
using Zenject;

namespace Game.Ingame
{
    public class LevelCameraInstaller : MonoInstaller<LevelCameraInstaller>
    {
        public override void InstallBindings()
        {
            Container.Bind<CinemachineVirtualCameraBase>().FromComponentInHierarchy().AsSingle();
            Container.Bind<CinemachineBrain>().FromComponentInHierarchy().AsSingle();
        }
    }
}
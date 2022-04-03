using UnityEngine;
using Zenject;

namespace Game.Ingame.Tank
{
    public class TankIngameInstaller : MonoInstaller<TankIngameInstaller>
    {
        public override void InstallBindings()
        {
            Container.Bind<TankController>().FromComponentOn(gameObject).AsSingle();
            Container.Bind<MeshRenderer>().FromComponentsInChildren().AsSingle();
        }
    }
}
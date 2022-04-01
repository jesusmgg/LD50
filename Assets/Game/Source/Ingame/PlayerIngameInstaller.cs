using UnityEngine.InputSystem;
using Zenject;

namespace Game.Ingame
{
    public class PlayerIngameInstaller : MonoInstaller<PlayerIngameInstaller>
    {
        public override void InstallBindings()
        {
            Container.Bind<PlayerInput>().FromComponentOn(gameObject).AsSingle();
        }
    }
}
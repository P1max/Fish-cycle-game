using Spawners;
using Zenject;

namespace Installers
{
    public class GameplaySceneInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<FishPool>()
                .FromComponentInHierarchy()
                .AsSingle();
        }
    }
}
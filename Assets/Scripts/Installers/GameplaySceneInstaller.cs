using Core.Feed;
using Spawners;
using UI.FeedJar;
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
            
            Container.Bind<FeedJarView>()
                .FromComponentInHierarchy()
                .AsSingle();
            
            Container.Bind<FoodPool>()
                .FromComponentInHierarchy()
                .AsSingle();
            
            Container.Bind<FeedJarPresenter>().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<FeedManager>().AsSingle();
        }
    }
}
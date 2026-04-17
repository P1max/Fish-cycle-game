using Core.Feed;
using Core.Game;
using Core.Game.Upgrade;
using Zenject;

namespace Installers
{
    public class GameplayCoreInstaller : Installer<GameplayCoreInstaller>
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<FeedManager>().AsSingle();
            Container.BindInterfacesAndSelfTo<FishesManager>().AsSingle();
            Container.BindInterfacesAndSelfTo<BalanceManager>().AsSingle();
            Container.BindInterfacesAndSelfTo<UpgradeManager>().AsSingle();
            Container.BindInterfacesAndSelfTo<BreedManager>().AsSingle();
            Container.BindInterfacesAndSelfTo<StoreManager>().AsSingle();

            Container.Bind<AquariumBoundsManager>().AsSingle();

            Container.BindInterfacesAndSelfTo<AquariumBounds>().FromComponentInHierarchy().AsSingle();
            Container.BindInterfacesAndSelfTo<WaterInteractionManager>().FromComponentInHierarchy().AsSingle();
        }
    }
}
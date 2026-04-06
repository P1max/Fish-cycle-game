using _Project.UI.AquariumUpgrader;
using UI;
using UI.Background;
using UI.Conveyor;
using UI.FeedJar;
using UI.MoneyCounter;
using Zenject;

namespace Installers
{
    public class UIInstaller : Installer<UIInstaller>
    {
        public override void InstallBindings()
        {
            Container.Bind<UI.Core.UIRoot>().FromComponentInHierarchy().AsSingle();
            Container.BindInterfacesAndSelfTo<UITankBounds>().FromComponentInHierarchy().AsSingle();

            Container.BindInterfacesAndSelfTo<FeedJarPresenter>().AsSingle();
            Container.Bind<FeedJarView>().FromComponentInHierarchy().AsSingle();

            Container.BindInterfacesAndSelfTo<FishesCounterPresenter>().AsSingle();
            Container.Bind<FishesCounterView>().FromComponentInHierarchy().AsSingle();

            Container.BindInterfacesAndSelfTo<CoinsCounterPresenter>().AsSingle();
            Container.Bind<CoinsCounterView>().FromComponentInHierarchy().AsSingle();

            Container.BindInterfacesAndSelfTo<AquariumUpgraderPresenter>().AsSingle();
            Container.Bind<AquariumUpgraderView>().FromComponentInHierarchy().AsSingle();

            Container.BindInterfacesAndSelfTo<BackgroundPresenter>().AsSingle();
            Container.Bind<BackgroundView>().FromComponentInHierarchy().AsSingle();

            Container.BindInterfacesAndSelfTo<ConveyorPresenter>().AsSingle();
            Container.BindInterfacesAndSelfTo<ConveyorView>().FromComponentInHierarchy().AsSingle();
        }
    }
}
using _Project.Core.States;
using _Project.UI.AquariumUpgrader;
using App;
using Core.Configs;
using Core.Configs.Providers;
using Core.Feed;
using Core.Game;
using Core.Game.Upgrade;
using Core.Loaders;
using Spawners;
using UI;
using UI.Background;
using UI.FeedJar;
using UI.MoneyCounter;
using UnityEngine;
using Zenject;

namespace Installers
{
    public class GameplaySceneInstaller : MonoInstaller
    {
        [SerializeField] private GameConfigProvider _configProvider;

        public override void InstallBindings()
        {
            Container.Bind<AquariumConfig>().FromInstance(_configProvider.GetAquariumConfig()).AsSingle();
            Container.Bind<FeederConfig>().FromInstance(_configProvider.GetFeederConfig()).AsSingle();
            Container.Bind<CommonFishConfig>().FromInstance(_configProvider.GetCommonFishConfig()).AsSingle();
            Container.Bind<ConveyorConfig>().FromInstance(_configProvider.GetConveyorConfig()).AsSingle();
            Container.Bind<UpgradesConfig>().FromInstance(_configProvider.GetUpgradesConfig()).AsSingle();

            Container.Bind<FishesConfigsLoader>().AsSingle().WithArguments(_configProvider.GetActiveFishes());

            Container.Bind<FishPool>().FromComponentInHierarchy().AsSingle();
            Container.Bind<CoinsPool>().FromComponentInHierarchy().AsSingle();
            Container.Bind<FoodPool>().FromComponentInHierarchy().AsSingle();
            Container.Bind<EffectsPool>().FromComponentInHierarchy().AsSingle();

            Container.Bind<FeedJarPresenter>().AsSingle().NonLazy();
            Container.Bind<FeedJarView>().FromComponentInHierarchy().AsSingle();

            Container.BindInterfacesAndSelfTo<FishesCounterPresenter>().AsSingle().NonLazy();
            Container.Bind<FishesCounterView>().FromComponentInHierarchy().AsSingle();

            Container.BindInterfacesAndSelfTo<CoinsCounterPresenter>().AsSingle().NonLazy();
            Container.Bind<CoinsCounterView>().FromComponentInHierarchy().AsSingle();

            Container.Bind<AquariumUpgraderPresenter>().AsSingle().NonLazy();
            Container.Bind<AquariumUpgraderView>().FromComponentInHierarchy().AsSingle();

            Container.Bind<BackgroundPresenter>().AsSingle().NonLazy();
            Container.Bind<BackgroundView>().FromComponentInHierarchy().AsSingle();

            Container.BindInterfacesAndSelfTo<FeedManager>().AsSingle();
            Container.Bind<FishesManager>().AsSingle().NonLazy();
            Container.Bind<BalanceManager>().AsSingle().NonLazy();
            Container.Bind<UpgradeManager>().AsSingle().NonLazy();

            Container.Bind<AquariumBounds>().FromComponentInHierarchy().AsSingle();
            Container.Bind<UITankBounds>().FromComponentInHierarchy().AsSingle();
            Container.Bind<WaterInteractionManager>().FromComponentInHierarchy().AsSingle();

            Container.Bind<AquariumBoundsManager>().AsSingle().NonLazy();
            Container.Bind<BreedManager>().AsSingle().NonLazy();

            Container.Bind<GameStateMachine>().AsSingle();
            Container.Bind<BootstrapState>().AsSingle();
            Container.Bind<PlayState>().AsSingle();

            Container.BindInterfacesTo<GameBootstrapper>().AsSingle();
        }
    }
}
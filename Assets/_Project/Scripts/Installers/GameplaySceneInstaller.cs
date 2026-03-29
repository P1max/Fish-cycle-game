using System.Linq;
using _Project.UI.AquariumUpgrader;
using Core.Boot;
using Core.Configs;
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
        [SerializeField] private GameConfigs _configs;

        public override void InstallBindings()
        {
            var aquariumConfig = _configs.Aquarium;
            var feederConfig = _configs.Feeder;
            var commonFishConfig = _configs.CommonFish;
            var conveyorConfig = _configs.Conveyor;
            var upgradesConfig = _configs.UpgradesConfig;

            var activeFishesList = _configs.FishesDatabase.Fishes;

            if (_configs.UseJsonConfig)
            {
                aquariumConfig = Instantiate(_configs.Aquarium);
                feederConfig = Instantiate(_configs.Feeder);
                commonFishConfig = Instantiate(_configs.CommonFish);
                conveyorConfig = Instantiate(_configs.Conveyor);
                activeFishesList = _configs.FishesDatabase.Fishes.Select(f => f.Clone()).ToList();
            }

            Container.Bind<GameConfigs>().FromInstance(_configs).AsSingle();
            Container.Bind<FeederConfig>().FromInstance(feederConfig).AsSingle();
            Container.Bind<AquariumConfig>().FromInstance(aquariumConfig).AsSingle();
            Container.Bind<ConveyorConfig>().FromInstance(conveyorConfig).AsSingle();
            Container.Bind<CommonFishConfig>().FromInstance(commonFishConfig).AsSingle();
            Container.Bind<UpgradesConfig>().FromInstance(upgradesConfig).AsSingle();

            Container.Bind<FishesLoader>().AsSingle().WithArguments(activeFishesList);

            Container.Bind<ConfigValidator>().AsSingle()
                .WithArguments(_configs, aquariumConfig, feederConfig, conveyorConfig, commonFishConfig).NonLazy();

            Container.Bind<FishPool>().FromComponentInHierarchy().AsSingle();
            Container.Bind<CoinsPool>().FromComponentInHierarchy().AsSingle();
            Container.Bind<FoodPool>().FromComponentInHierarchy().AsSingle();
            Container.Bind<EffectsPool>().FromComponentInHierarchy().AsSingle();

            Container.Bind<FeedJarPresenter>().AsSingle().NonLazy();
            Container.Bind<FeedJarView>().FromComponentInHierarchy().AsSingle();

            Container.Bind<FishesCounterPresenter>().AsSingle().NonLazy();
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

            Container.BindInterfacesTo<GameBootstrapper>().AsSingle();
        }
    }
}
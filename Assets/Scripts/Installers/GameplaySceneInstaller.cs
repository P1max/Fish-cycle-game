using Core.Boot;
using Core.Configs;
using Core.Feed;
using Core.Game;
using Core.Loaders;
using Core.Profile;
using Spawners;
using UI;
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
            Container.Bind<GameConfigs>().FromInstance(_configs).AsSingle();

            Container.Bind<FeederConfig>().FromInstance(_configs.Feeder).AsSingle();
            Container.Bind<AquariumConfig>().FromInstance(_configs.Aquarium).AsSingle();
            Container.Bind<CommonFishConfig>().FromInstance(_configs.CommonFish).AsSingle();

            Container.Bind<FishesLoader>().AsSingle();
            Container.Bind<ConfigValidator>().AsSingle().NonLazy();

            Container.Bind<PlayerProfile>().AsSingle();

            Container.Bind<FishPool>().FromComponentInHierarchy().AsSingle();
            Container.Bind<CoinsPool>().FromComponentInHierarchy().AsSingle();
            Container.Bind<FoodPool>().FromComponentInHierarchy().AsSingle();

            Container.Bind<FeedJarPresenter>().AsSingle().NonLazy();
            Container.Bind<FeedJarView>().FromComponentInHierarchy().AsSingle();

            Container.Bind<FishesCounterPresenter>().AsSingle().NonLazy();
            Container.Bind<FishesCounterView>().FromComponentInHierarchy().AsSingle();

            Container.Bind<CoinsCounterPresenter>().AsSingle().NonLazy();
            Container.Bind<CoinsCounterView>().FromComponentInHierarchy().AsSingle();

            Container.BindInterfacesAndSelfTo<FeedManager>().AsSingle();
            Container.Bind<FishesManager>().AsSingle().NonLazy();
            Container.Bind<BalanceManager>().AsSingle().NonLazy();

            Container.BindInterfacesTo<GameBootstrapper>().AsSingle();
        }
    }
}
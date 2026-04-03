using _Project.Core.States;
using _Project.UI.AquariumUpgrader;
using App;
using Core.Configs;
using Core.Feed;
using Core.Game;
using Core.Game.Upgrade;
using Core.Loaders;
using Features.BotBalancer.AI;
using Features.BotBalancer.Analytics;
using Spawners;
using UI;
using UI.Background;
using UI.Conveyor;
using UI.FeedJar;
using UI.MoneyCounter;
using UnityEngine;
using Zenject;

namespace Installers
{
    public class GameplaySceneInstaller : MonoInstaller
    {
        [SerializeField] private DataSource _dataSource;

        public override void InstallBindings()
        {
            BindConfigs();
            BindPools();
            BindBot();
            BindGameplayCore();
            BindUI();
            BindStateMachine();
        }

        private void BindConfigs()
        {
            var provider = _dataSource.ActiveProvider;

            if (provider == null)
            {
                Debug.LogError("Active Provider is not set in GameSettings");

                return;
            }

            Container.Bind<AquariumConfig>().FromInstance(provider.GetAquariumConfig()).AsSingle();
            Container.Bind<FeederConfig>().FromInstance(provider.GetFeederConfig()).AsSingle();
            Container.Bind<CommonFishConfig>().FromInstance(provider.GetCommonFishConfig()).AsSingle();
            Container.Bind<ConveyorConfig>().FromInstance(provider.GetConveyorConfig()).AsSingle();
            Container.Bind<UpgradesConfig>().FromInstance(provider.GetUpgradesConfig()).AsSingle();

            Container.Bind<FishesConfigsLoader>().AsSingle().WithArguments(provider.GetActiveFishes());
        }

        private void BindPools()
        {
            Container.BindInterfacesAndSelfTo<FishPool>().FromComponentInHierarchy().AsSingle();
            Container.BindInterfacesAndSelfTo<CoinsPool>().FromComponentInHierarchy().AsSingle();
            Container.BindInterfacesAndSelfTo<FoodPool>().FromComponentInHierarchy().AsSingle();
            Container.BindInterfacesAndSelfTo<EffectsPool>().FromComponentInHierarchy().AsSingle();
        }

        private void BindGameplayCore()
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

        private void BindBot()
        {
            Container.BindInterfacesAndSelfTo<Features.BotBalancer.BotSetupService>().AsSingle();

            var isBotActive = false;

#if UNITY_EDITOR
            isBotActive = UnityEditor.EditorPrefs.GetBool("Bot_IsActive", false);
#endif

            if (!isBotActive) return;

            BotProfileConfig activeProfile = null;

#if UNITY_EDITOR
            var profilePath = UnityEditor.EditorPrefs.GetString("Bot_ProfilePath", "");

            if (!string.IsNullOrEmpty(profilePath)) 
                activeProfile = UnityEditor.AssetDatabase.LoadAssetAtPath<BotProfileConfig>(profilePath);
#endif

            if (activeProfile)
            {
                Debug.Log($"Для симуляции загружен профиль: {activeProfile.name}");
            }
            else
            {
                Debug.LogWarning("Профиль из окна симуляции не найден. Бот не будет запущен.");

                return;
            }

            Container.Bind<BotProfileConfig>().FromInstance(activeProfile).AsSingle();

            Container.Bind<IBotAction>().To<FeedFishesAction>().AsSingle();
            Container.Bind<IBotAction>().To<BuyFishAction>().AsSingle();
            Container.Bind<IBotAction>().To<CollectCoinsAction>().AsSingle();
            Container.Bind<IBotAction>().To<UpgradeAquariumAction>().AsSingle();
            Container.Bind<IBotAction>().To<CollectDeadFishAction>().AsSingle();

            Container.BindInterfacesAndSelfTo<BotController>().AsSingle();
            Container.BindInterfacesAndSelfTo<BotAnalyticsService>().AsSingle();
        }

        private void BindUI()
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

        private void BindStateMachine()
        {
            Container.Bind<GameStateMachine>().AsSingle();
            Container.Bind<BootstrapState>().AsSingle();
            Container.Bind<PlayState>().AsSingle();

            Container.Bind<IInitializable>().To<GameBootstrapper>().AsSingle();
        }
    }
}
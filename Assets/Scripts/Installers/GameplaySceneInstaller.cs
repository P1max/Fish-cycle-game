using System.Collections.Generic;
using Core.Configs;
using Core.Feed;
using Spawners;
using UI.FeedJar;
using UnityEngine;
using Zenject;

namespace Installers
{
    public class GameplaySceneInstaller : MonoInstaller
    {
        [SerializeField] private GameConfigs _configs;

        public override void InstallBindings()
        {
            var configValidator = new ConfigValidator(_configs);
            
            configValidator.LoadAndPatchFromJson();
            
            Container.Bind<Dictionary<string, FishConfig>>().FromInstance(configValidator.LoadedFishes).AsSingle();
            Container.Bind<FeederConfig>().FromInstance(_configs.Feeder).AsSingle();
            Container.Bind<AquariumConfig>().FromInstance(_configs.Aquarium).AsSingle();
            Container.Bind<CommonFishConfig>().FromInstance(_configs.CommonFish).AsSingle();

            Container.Bind<FishPool>().FromComponentInHierarchy().AsSingle();
            Container.Bind<FeedJarView>().FromComponentInHierarchy().AsSingle();
            Container.Bind<FoodPool>().FromComponentInHierarchy().AsSingle();

            Container.Bind<FeedJarPresenter>().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<FeedManager>().AsSingle();
        }
    }
}
using Core.Configs;
using Core.Loaders;
using UnityEngine;
using Zenject;

namespace Installers
{
    public class ConfigsInstaller : Installer<DataSource, ConfigsInstaller>
    {
        private readonly DataSource _dataSource;

        public ConfigsInstaller(DataSource dataSource)
        {
            _dataSource = dataSource;
        }

        public override void InstallBindings()
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
    }
}
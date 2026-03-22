using Core.Game;
using Core.Loaders;
using UnityEngine;
using Zenject;

namespace Core.Boot
{
    public class GameBootstrapper : IInitializable
    {
        private readonly FishesManager _fishesManager;
        private readonly FishesLoader _fishesLoader;
        private readonly AquariumConfig _aquariumConfig;

        public GameBootstrapper(
            FishesManager fishesManager,
            FishesLoader fishesLoader,
            AquariumConfig aquariumConfig)
        {
            _fishesManager = fishesManager;
            _fishesLoader = fishesLoader;
            _aquariumConfig = aquariumConfig;
        }

        public void Initialize()
        {
            Debug.Log("Старт игры");

            SpawnInitialFishes();
        }

        private void SpawnInitialFishes()
        {
            var fishesToSpawn = _aquariumConfig.StartFishCount;
            var fishConfigs = _fishesLoader.LoadedFishesList;

            if (fishConfigs.Count == 0)
            {
                Debug.LogError("Нет доступных конфигов рыбок для спавна");

                return;
            }

            for (var i = 0; i < fishesToSpawn; i++)
            {
                var randomIndex = Random.Range(0, fishConfigs.Count);
                var fishConfig = fishConfigs[randomIndex];

                _fishesManager.TryAddFish(fishConfig.Id);
            }
        }
    }
}
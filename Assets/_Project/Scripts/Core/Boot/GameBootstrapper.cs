using System.Linq;
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
            var fishesIdList = _fishesLoader.LoadedFishesDict.Keys.ToList();

            if (fishesIdList.Count == 0)
            {
                Debug.LogError("Нет доступных конфигов рыбок для спавна");

                return;
            }

            for (var i = 0; i < fishesToSpawn; i++)
            {
                var randomIndex = Random.Range(0, fishesIdList.Count);
                var fishId = fishesIdList[randomIndex];
                var fishConfig = _fishesLoader.LoadedFishesDict[fishId];

                _fishesManager.TryAddFish(fishConfig.Id);
            }
        }
    }
}
using System.Linq;
using Core.Game;
using Core.Loaders;
using UnityEngine;

namespace _Project.Core.States
{
    public class PlayState : IGameState
    {
        private readonly FishesManager _fishesManager;
        private readonly FishesConfigsLoader _fishesLoader;
        private readonly AquariumConfig _aquariumConfig;

        public PlayState(FishesManager fishesManager, FishesConfigsLoader fishesLoader, AquariumConfig aquariumConfig)
        {
            _fishesManager = fishesManager;
            _fishesLoader = fishesLoader;
            _aquariumConfig = aquariumConfig;
        }

        private void SpawnInitialFishes()
        {
            var fishesIdList = _fishesLoader.LoadedFishesDict.Keys.ToList();

            if (fishesIdList.Count == 0) return;

            for (var i = 0; i < _aquariumConfig.StartFishCount; i++)
            {
                var fishId = fishesIdList[Random.Range(0, fishesIdList.Count)];

                _fishesManager.TryAddFish(fishId);
            }
        }

        public void Enter()
        {
            Debug.Log("Старт игры");

            SpawnInitialFishes();
        }

        public void Exit()
        {
        }
    }
}
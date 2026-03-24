using System;
using Spawners;
using UnityEngine;

namespace Core.Game
{
    public class FishesManager
    {
        private readonly FishPool _fishPool;
        private readonly AquariumConfig _config;

        public event Action<int> OnFishCountChanged;
        
        public int CurrentFishCount { get; private set; }

        public FishesManager(FishPool fishPool, AquariumConfig config)
        {
            _fishPool = fishPool;
            _config = config;
        }

        public bool CanAddFish => CurrentFishCount < _config.MaxFishCount;

        public bool TryAddFish(string fishId, Vector2 spawnPosition = default)
        {
            if (!CanAddFish) return false;

            var fish = _fishPool.GetFish(fishId);
            
            fish.transform.position = spawnPosition;
            fish.OnReadyToPool += HandleFishDeath;

            CurrentFishCount++;
            OnFishCountChanged?.Invoke(CurrentFishCount);
            
            return true;
        }

        private void HandleFishDeath(FishEntity fish)
        {
            fish.OnReadyToPool -= HandleFishDeath;
            
            CurrentFishCount--;
            OnFishCountChanged?.Invoke(CurrentFishCount);
        }
    }
}
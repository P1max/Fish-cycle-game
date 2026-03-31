using System;
using _Project.Core.Interfaces;
using Core.Game.Upgrade;
using Spawners;
using UnityEngine;

namespace Core.Game
{
    public class FishesManager : IGameplayInit, IDisposable
    {
        private readonly FishPool _fishPool;
        private readonly AquariumConfig _config;
        private readonly UpgradeManager _upgradeManager;

        public event Action<int, int> OnFishCountChanged;

        public int CurrentFishCount { get; private set; }
        public int MaxFishesCount { get; private set; }

        public bool CanAddFish => CurrentFishCount < MaxFishesCount;

        public FishesManager(FishPool fishPool, AquariumConfig config, UpgradeManager upgradeManager)
        {
            _fishPool = fishPool;
            _config = config;
            _upgradeManager = upgradeManager;
        }

        private void HandleAquariumUpgrade(UpgradesConfig.LevelData upgradeData)
        {
            MaxFishesCount = upgradeData.NewMaxFishesCount;

            OnFishCountChanged?.Invoke(CurrentFishCount, MaxFishesCount);
        }

        private void HandleFishDeath(FishEntity fish)
        {
            fish.OnReadyToPool -= HandleFishDeath;

            CurrentFishCount--;
            OnFishCountChanged?.Invoke(CurrentFishCount, MaxFishesCount);
        }

        public bool TryAddFish(string fishId, float quality = 1f, Vector2 spawnPosition = default)
        {
            if (!CanAddFish) return false;

            var fish = _fishPool.Get();

            fish.SetConfig(fishId, quality);
            fish.transform.position = spawnPosition;
            fish.OnReadyToPool += HandleFishDeath;

            CurrentFishCount++;
            OnFishCountChanged?.Invoke(CurrentFishCount, MaxFishesCount);

            return true;
        }

        public void Init()
        {
            MaxFishesCount = _config.MaxFishCount;

            _upgradeManager.OnAquariumUpgrade += HandleAquariumUpgrade;
        }

        public void Dispose()
        {
            _upgradeManager.OnAquariumUpgrade -= HandleAquariumUpgrade;
        }
    }
}
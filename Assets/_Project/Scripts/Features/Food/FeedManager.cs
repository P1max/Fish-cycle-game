using System;
using _Project.Core.Interfaces;
using Core.Game.Upgrade;
using Spawners;
using UnityEngine;
using Zenject;

namespace Core.Feed
{
    public class FeedManager : IGameplayInit, IDisposable, ITickable
    {
        private readonly FoodPool _foodPool;
        private readonly FeederConfig _config;
        private readonly UpgradeManager _upgradeManager;

        private bool _isReady;
        private float _currentTime;

        private float _activeCooldown;
        private float _nextCooldown;

        public event Action<float> OnNormalizedTime;
        
        public event Action OnFeederUsed;

        public float ActiveCooldown => _activeCooldown;

        public bool IsReady => _isReady;
        
        public FeedManager(FoodPool foodPool, FeederConfig config, UpgradeManager upgradeManager)
        {
            _foodPool = foodPool;
            _config = config;
            _upgradeManager = upgradeManager;
        }

        private void HandleUpgrade(UpgradesConfig.LevelData upgradeData) => _nextCooldown = upgradeData.NewFeederCooldown;

        public bool TryFeed()
        {
            if (!_isReady) return false;

            var cam = Camera.main;

            if (cam == null) return false;

            var foodAmount = UnityEngine.Random.Range(_config.FoodPiecesCount.x, _config.FoodPiecesCount.y + 1) +
                             (_upgradeManager.CurrentLevelData?.FoodCountIncrementToDefault ?? 0);

            var nutritionPerPiece = (_config.TotalHungerRestorePerUse +
                                     (_upgradeManager.CurrentLevelData?.HungerRestorePerUseIncrementToDefault ?? 0)) / foodAmount;

            var topLeft = cam.ViewportToWorldPoint(new Vector3(0, 1, 0));
            var bottomRight = cam.ViewportToWorldPoint(new Vector3(1, 0, 0));

            for (var i = 0; i < foodAmount; i++)
            {
                var food = _foodPool.Get();

                var randomX = UnityEngine.Random.Range(topLeft.x + 0.5f, bottomRight.x - 0.5f);
                var startY = topLeft.y + 0.2f;
                var delay = (i == 0) ? 0f : UnityEngine.Random.Range(0.4f, 1.2f);

                food.Spawn(new Vector2(randomX, startY), nutritionPerPiece, delay);
            }

            Reset();
            
            OnFeederUsed?.Invoke();

            return true;
        }

        public void Reset()
        {
            _activeCooldown = _nextCooldown;

            _isReady = false;
            _currentTime = 0;

            OnNormalizedTime?.Invoke(0f);
        }

        public void Tick()
        {
            if (_isReady) return;

            _currentTime += Time.deltaTime;

            OnNormalizedTime?.Invoke(Mathf.Clamp(_currentTime / _activeCooldown, 0, 1));

            if (_currentTime >= _activeCooldown) _isReady = true;
        }

        public void Init()
        {
            _nextCooldown = _config.CooldownSeconds;

            _upgradeManager.OnAquariumUpgrade += HandleUpgrade;

            Reset();
        }

        public void Dispose()
        {
            _upgradeManager.OnAquariumUpgrade -= HandleUpgrade;
        }
    }
}
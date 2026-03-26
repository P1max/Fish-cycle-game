using System;
using Spawners;
using UnityEngine;
using Zenject;

namespace Core.Feed
{
    public class FeedManager : ITickable
    {
        private readonly FoodPool _foodPool;
        private readonly FeederConfig _config;

        private bool _isReady;
        private float _currentTime;

        public event Action<float> _normalizedTime;
        
        public FeedManager(FoodPool foodPool, FeederConfig config)
        {
            _foodPool = foodPool;
            _config = config;

            Reset();
        }

        public bool TryFeed()
        {
            if (!_isReady) return false;    

            var cam = Camera.main;

            if (cam == null) return false;

            var foodAmount = UnityEngine.Random.Range(_config.FoodPiecesCount.x, _config.FoodPiecesCount.y + 1);
            var nutritionPerPiece = _config.TotalHungerRestorePerUse / foodAmount;

            var topLeft = cam.ViewportToWorldPoint(new Vector3(0, 1, 0));
            var bottomRight = cam.ViewportToWorldPoint(new Vector3(1, 0, 0));

            for (var i = 0; i < foodAmount; i++)
            {
                var food = _foodPool.Get();

                var randomX = UnityEngine.Random.Range(topLeft.x + 0.5f, bottomRight.x - 0.5f);
                var startY = topLeft.y + 0.2f;
                var targetY = bottomRight.y - 0.5f;
                var delay = (i == 0) ? 0f : UnityEngine.Random.Range(0.4f, 1.2f);

                food.Spawn(new Vector2(randomX, startY), nutritionPerPiece, delay);
            }

            Reset();

            return true;
        }

        public void Reset()
        {
            _isReady = false;
            _currentTime = 0;

            _normalizedTime?.Invoke(0f);
        }

        public void Tick()
        {
            if (_isReady) return;

            _currentTime += Time.deltaTime;

            _normalizedTime?.Invoke(Mathf.Clamp(_currentTime / _config.CooldownSeconds, 0, 1));

            if (_currentTime >= _config.CooldownSeconds) _isReady = true;
        }
    }
}
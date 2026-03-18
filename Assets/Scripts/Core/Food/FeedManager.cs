using System;
using Spawners;
using UnityEngine;
using Zenject;

namespace Core.Feed
{
    public class FeedManager : ITickable
    {
        private readonly FoodPool _foodPool;

        private bool _isReady;
        private float _timer;
        private float _currentTime;
        private float _totalNutritionPerUse;

        public event Action<float> _normalizedTime;

        public bool IsReady => _isReady;

        public FeedManager(FoodPool foodPool)
        {
            _foodPool = foodPool;

            _timer = 8f;
            _totalNutritionPerUse = 45f;

            Reset();
        }

        public void TryFeed()
        {
            if (!_isReady) return;

            var cam = Camera.main;
            if (cam == null) return;

            var foodAmount = UnityEngine.Random.Range(3, 7);
            var nutritionPerPiece = _totalNutritionPerUse / foodAmount; 

            var topLeft = cam.ViewportToWorldPoint(new Vector3(0, 1, 0));
            var bottomRight = cam.ViewportToWorldPoint(new Vector3(1, 0, 0));

            for (var i = 0; i < foodAmount; i++)
            {
                var food = _foodPool.GetFood();

                var randomX = UnityEngine.Random.Range(topLeft.x + 0.5f, bottomRight.x - 0.5f);
                var startY = topLeft.y + 0.2f;
                var targetY = bottomRight.y - 0.5f;
                var delay = (i == 0) ? 0f : UnityEngine.Random.Range(0.1f, 1.2f);

                food.Spawn(new Vector2(randomX, startY), targetY, nutritionPerPiece, delay);
            }

            Reset();
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

            _normalizedTime?.Invoke(Mathf.Clamp(_currentTime / _timer, 0, 1));

            if (_currentTime >= _timer) _isReady = true;
        }
    }
}
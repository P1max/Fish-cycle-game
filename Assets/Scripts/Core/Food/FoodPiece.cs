using DG.Tweening;
using UnityEngine;
using Spawners;

namespace Core.Feed
{
    [RequireComponent(typeof(Collider2D))]
    public class FoodPiece : MonoBehaviour
    {
        private FoodPool _pool;
        private bool _isConsumed;

        public float NutritionValue { get; private set; }
        
        public bool IsConsumed => _isConsumed;

        private void OnTriggerEnter2D(Collider2D col)
        {
            if (_isConsumed) return;

            if (col.TryGetComponent<FishEntity>(out var fish)) Consume(fish);
        }

        private void Consume(FishEntity fish)
        {
            _isConsumed = true;

            Debug.Log($"Скушана");
            fish.Hunger.Feed(NutritionValue);

            transform.DOScale(Vector3.zero, 0.2f).OnComplete(ReturnToPool);
        }

        private void ReturnToPool()
        {
            transform.DOKill();

            _pool.ReturnFood(this);
        }

        public void Spawn(Vector2 startPosition, float targetBottomY, float nutritionValue, float delay)
        {
            _isConsumed = false;
            transform.position = startPosition;
            NutritionValue = nutritionValue;

            transform.localScale = Vector3.zero;

            var sequence = DOTween.Sequence();

            sequence.SetDelay(delay);
            sequence.Append(transform.DOScale(Vector3.one, 0.4f).SetEase(Ease.OutBack));

            var waterHitY = startPosition.y - 1.2f;
            
            sequence.Join(transform.DOMoveY(waterHitY, 0.6f).SetEase(Ease.OutCubic));

            var fallDuration = Random.Range(6f, 9f);
            
            sequence.Append(transform.DOMoveY(targetBottomY, fallDuration).SetEase(Ease.Linear));

            sequence.OnComplete(() =>
            {
                if (!_isConsumed) ReturnToPool();
            });
        }

        public void Init(FoodPool pool)
        {
            _pool = pool;
        }
    }
}
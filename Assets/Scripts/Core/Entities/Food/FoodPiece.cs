using DG.Tweening;
using UnityEngine;
using Spawners;
using Random = UnityEngine.Random;

namespace Core.Feed
{
    [RequireComponent(typeof(Collider2D))]
    public class FoodPiece : MonoBehaviour
    {
        private FoodPool _pool;
        private bool _isConsumed;
        private float _nutritionValue;
        private Rigidbody2D _rigidbody;
        private Collider2D _collider;
        private float _startScale;

        public bool IsConsumed => _isConsumed;

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody2D>();
            _collider = GetComponent<Collider2D>();
            _startScale = transform.localScale.x;
        }

        private void ReturnToPool()
        {
            transform.DOKill();

            _pool.ReturnFood(this);
        }

        public void Consume(FishEntity fish)
        {
            if (_isConsumed) return;

            _isConsumed = true;

            _rigidbody.bodyType = RigidbodyType2D.Kinematic;
            _rigidbody.linearVelocity = Vector2.zero;

            fish.Hunger.Feed(_nutritionValue);

            transform.DOScale(Vector3.zero, 0.2f).OnComplete(ReturnToPool);
        }

        public void Spawn(Vector2 startPosition, float nutritionValue, float delay)
        {
            _isConsumed = false;
            transform.position = startPosition;
            _nutritionValue = nutritionValue;
            transform.localScale = Vector3.zero;

            _rigidbody.bodyType = RigidbodyType2D.Kinematic;
            _rigidbody.linearVelocity = Vector2.zero;

            var sequence = DOTween.Sequence();

            sequence.SetTarget(transform);
            sequence.SetDelay(delay);
            sequence.Append(transform.DOScale(_startScale, 0.3f).SetEase(Ease.OutBack));

            var waterHitY = startPosition.y - 1.6f;

            sequence.Join(transform.DOMoveY(waterHitY, 0.6f).SetEase(Ease.OutCubic));

            sequence.AppendCallback(() =>
            {
                _rigidbody.bodyType = RigidbodyType2D.Dynamic;

                _rigidbody.AddForce(new Vector2(Random.Range(-0.02f, 0.02f), 0), ForceMode2D.Impulse);
            });

            sequence.AppendInterval(30f);

            sequence.OnComplete(() =>
            {
                if (!_isConsumed)
                {
                    _isConsumed = true;
                    transform.DOScale(Vector3.zero, 0.2f).OnComplete(ReturnToPool);
                }
            });
        }

        public void Init(FoodPool pool)
        {
            _pool = pool;
        }
    }
}
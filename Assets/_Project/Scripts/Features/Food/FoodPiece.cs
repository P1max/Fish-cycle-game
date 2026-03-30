using DG.Tweening;
using UnityEngine;
using Spawners;
using Random = UnityEngine.Random;

namespace Core.Feed
{
    public class FoodPiece : MonoBehaviour
    {
        [SerializeField] private float _lifeTime = 30f;

        private bool _isConsumed;
        private float _startScale;
        private float _nutritionValue;
        private FoodPool _pool;
        private Sequence _spawnSequence;
        private Rigidbody2D _rigidbody;

        public bool IsConsumed => _isConsumed;

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody2D>();
            _startScale = transform.localScale.x;
        }
        
        private void RemoveFromAquarium()
        {
            _isConsumed = true;

            transform.DOScale(Vector3.zero, 0.2f)
                .OnComplete(() => _pool.ReturnToPool(this));
        }

        public void Consume(FishEntity fish)
        {
            if (_isConsumed) return;

            _isConsumed = true;

            _spawnSequence?.Kill();
            transform.DOKill();

            _rigidbody.bodyType = RigidbodyType2D.Kinematic;
            _rigidbody.linearVelocity = Vector2.zero;

            fish.Hunger.Feed(_nutritionValue);

            RemoveFromAquarium();
        }

        public void Spawn(Vector2 startPosition, float nutritionValue, float delay)
        {
            _isConsumed = false;
            _nutritionValue = nutritionValue;

            _spawnSequence?.Kill();
            transform.DOKill();

            transform.position = startPosition;
            transform.localScale = Vector3.zero;

            _rigidbody.bodyType = RigidbodyType2D.Kinematic;
            _rigidbody.linearVelocity = Vector2.zero;

            var waterHitY = startPosition.y - 1.6f;

            _spawnSequence = DOTween.Sequence()
                .SetDelay(delay)
                .Append(transform.DOScale(_startScale, 0.3f).SetEase(Ease.OutBack))
                .Join(transform.DOMoveY(waterHitY, 0.6f).SetEase(Ease.OutCubic))
                .AppendCallback(() =>
                {
                    _rigidbody.bodyType = RigidbodyType2D.Dynamic;
                    _rigidbody.AddForce(new Vector2(Random.Range(-0.02f, 0.02f), 0f), ForceMode2D.Impulse);
                })
                .AppendInterval(_lifeTime)
                .OnComplete(RemoveFromAquarium);
        }

        public void Init(FoodPool pool)
        {
            _pool = pool;
        }
    }
}
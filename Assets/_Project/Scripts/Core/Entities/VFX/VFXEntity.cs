using DG.Tweening;
using Spawners;
using UnityEngine;

namespace Core.Entities.VFX
{
    public class VFXEntity : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer _spriteRenderer;

        private EffectsPool _pool;
        private Vector3 _defaultScale;

        private Sequence _sequence;

        private void Awake()
        {
            _defaultScale = transform.localScale;
        }

        public void Init(EffectsPool pool)
        {
            _pool = pool;
        }

        public void Play(Vector2 position)
        {
            transform.position = position;

            _sequence.Kill();
            transform.DOKill();

            _spriteRenderer.color = Color.white;
            transform.localScale = Vector3.zero;

            _sequence = DOTween.Sequence()
                .Append(transform.DOScale(_defaultScale, 0.8f).SetEase(Ease.OutBack))
                .Join(transform.DOMoveY(position.y + 1.5f, 1.5f).SetEase(Ease.OutQuad))
                .Join(_spriteRenderer.DOFade(0f, 1.5f).SetEase(Ease.InQuad))
                .OnComplete(() => _pool.ReturnToPool(this));
        }
    }
}
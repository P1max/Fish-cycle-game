using DG.Tweening;
using UnityEngine;

namespace Core.Entities.VFX
{
    public class BirthVFXEntity : BaseVFXEntity
    {
        [SerializeField] private SpriteRenderer _symbolRenderer;
        [SerializeField] private ParticleSystem _heartsParticles;

        private Vector3 _defaultSymbolScale;
        private Sequence _sequence;

        [Header("Animation Settings")]
        [SerializeField] private float _animationDuration = 3f;
        [SerializeField] private float _floatUpHeight = 1.5f;

        private void Awake()
        {
            _defaultSymbolScale = _symbolRenderer.transform.localScale;
        }

        public override void Play(Vector2 position)
        {
            transform.position = position;

            _sequence?.Kill();
            _symbolRenderer.transform.DOKill();

            _symbolRenderer.color = Color.white;
            _symbolRenderer.transform.localScale = Vector3.zero;

            var emission = _heartsParticles.emission;
            emission.enabled = true;
            _heartsParticles.Play();

            var targetPosition = new Vector3(position.x, position.y + _floatUpHeight, transform.position.z);

            _sequence = DOTween.Sequence()
                .Append(_symbolRenderer.transform.DOScale(_defaultSymbolScale, 0.5f).SetEase(Ease.OutBack))
                .Join(transform.DOMove(targetPosition, _animationDuration).SetEase(Ease.OutQuad))
                .Join(_symbolRenderer.DOFade(0f, _animationDuration).SetEase(Ease.InQuad))
                .OnComplete(() =>
                {
                    emission.enabled = false;
                    ReturnToPool(); // Вызываем метод базового класса
                });
        }
    }
}
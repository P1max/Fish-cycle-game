using System;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using TMPro;
using Random = UnityEngine.Random;

namespace Core.Fish.Modules.Visual
{
    public class FishVisual : MonoBehaviour
    {
        private static readonly int _grayscaleAmount = Shader.PropertyToID("_GrayscaleAmount");

        [FoldoutGroup("Visual References")]
        [SerializeField] private Transform _visualTransform;

        [FoldoutGroup("Visual References")]
        [SerializeField] private SpriteRenderer _spriteRenderer;

        [FoldoutGroup("VFX (Bubbles)")]
        [SerializeField] private ParticleSystem _bubbleParticles;

        [FoldoutGroup("VFX (Bubbles)")]
        [MinMaxSlider(1f, 20f, true)]
        [LabelText("Интервал пузырьков (сек)")]
        [SerializeField] private Vector2 _bubblesTimer = new(3f, 5f);

        [FoldoutGroup("UI Indicators")]
        [SerializeField] private GameObject _hungryContainer;

        [FoldoutGroup("UI Indicators")]
        [SerializeField] private GameObject _deathContainer;

        [FoldoutGroup("UI Indicators")]
        [SerializeField] private TextMeshPro _deathText;

        private FishEntity _fishEntity;
        private Sequence _sequence;
        private Tween _bubbleTimer;

        public GameObject HungryContainer => _hungryContainer;
        public GameObject DeathContainer => _deathContainer;
        public TextMeshPro DeathText => _deathText;
        public float PoopAnimPreDelay => 0.35f;

        public void Init(FishEntity fishEntity)
        {
            _fishEntity = fishEntity;
        }

        private void Update()
        {
            if (_fishEntity == null || !_fishEntity.IsAlive) return;

            UpdateVisuals();
        }

        public void ResetVisuals()
        {
            _sequence?.Kill();
            _sequence = null;

            _visualTransform.localRotation = Quaternion.identity;
            _visualTransform.localPosition = Vector3.zero;
            _visualTransform.localScale = Vector3.one;

            _spriteRenderer.color = Color.white;
            _spriteRenderer.SetPropertyBlock(null);

            StartBubbles();
        }

        public void SetSprite(Sprite sprite)
        {
            _spriteRenderer.sprite = sprite;
        }

        public void PlaySpawnAnimation(Vector3 targetScale)
        {
            _sequence?.Complete();
            _sequence?.Kill();

            _fishEntity.transform.localScale = Vector3.zero;

            _sequence = DOTween.Sequence()
                .Append(_fishEntity.transform.DOScale(targetScale, 0.3f).SetEase(Ease.OutQuad))
                .Append(_fishEntity.transform.DOPunchScale(
                    new Vector3(targetScale.x * -0.3f, targetScale.y * -0.3f, 0f),
                    duration: 0.5f,
                    vibrato: 5,
                    elasticity: 1f
                ))
                .OnComplete(() => _fishEntity.transform.localScale = targetScale);

            _sequence.Play();
        }

        public void PlayPoopCoinAnimation(Action onCoinEject)
        {
            if (_fishEntity == null || !_fishEntity.IsAlive) return;

            _sequence?.Complete();
            _sequence?.Kill();

            var sign = Mathf.Sign(_visualTransform.localScale.x);
            var defaultScale = new Vector3(sign, 1f, 1f);

            _sequence = DOTween.Sequence()
                .Append(_visualTransform.DOScale(new Vector3(sign * 0.8f, 1.2f, 1f), 0.2f).SetEase(Ease.OutQuad))
                .Append(_visualTransform.DOScale(new Vector3(sign * 1.2f, 0.8f, 1f), 0.15f).SetEase(Ease.OutQuad))
                .AppendCallback(() => onCoinEject?.Invoke())
                .Append(_visualTransform.DOScale(defaultScale, 0.2f).SetEase(Ease.OutBack));

            _sequence.Play();
        }

        public void SetDeadVisuals()
        {
            StopBubbles();

            var grayscaleValue = 0f;
            var mpb = new MaterialPropertyBlock();
            _spriteRenderer.GetPropertyBlock(mpb);

            _sequence = DOTween.Sequence()
                .Append(_visualTransform.DORotate(new Vector3(0, 0, 180f), 1.5f))
                .Join(DOTween.To(() => grayscaleValue, x =>
                {
                    grayscaleValue = x;
                    mpb.SetFloat(_grayscaleAmount, grayscaleValue);
                    _spriteRenderer.SetPropertyBlock(mpb);
                }, 1f, 1.5f));

            _sequence.Play();
        }

        public void PlayCollectAnimation(Action onComplete)
        {
            StopBubbles();

            _sequence?.Complete();
            _sequence?.Kill();

            _sequence = DOTween.Sequence()
                .Append(_visualTransform.DOLocalMoveY(_visualTransform.localPosition.y + 1f, 0.5f))
                .Join(_spriteRenderer.DOFade(0f, 0.5f))
                .OnComplete(() => onComplete?.Invoke());

            _sequence.Play();
        }

        private void StartBubbles()
        {
            _bubbleTimer?.Kill();
            ScheduleNextBubble();
        }

        private void StopBubbles()
        {
            _bubbleTimer?.Kill();
        }

        private void ScheduleNextBubble()
        {
            if (_fishEntity == null || !_fishEntity.IsAlive) return;

            var randomDelay = Random.Range(_bubblesTimer.x, _bubblesTimer.y);

            _bubbleTimer = DOVirtual.DelayedCall(randomDelay, () =>
            {
                if (_fishEntity == null || !_fishEntity.IsAlive) return;

                _bubbleParticles.Emit(1);

                ScheduleNextBubble();
            });
        }

        private void UpdateVisuals()
        {
            if (_fishEntity.Movement.Velocity.x > 0.3f)
                _visualTransform.localScale = new Vector3(-1 * Mathf.Abs(_visualTransform.localScale.x), _visualTransform.localScale.y,
                    _visualTransform.localScale.z);
            else if (_fishEntity.Movement.Velocity.x < -0.3f)
                _visualTransform.localScale = new Vector3(1 * Mathf.Abs(_visualTransform.localScale.x), _visualTransform.localScale.y,
                    _visualTransform.localScale.z);

            var targetAngleZ = Mathf.Clamp(
                (_fishEntity.Movement.Velocity.y / _fishEntity.Config.NormalSpeedRange.y) * _fishEntity.Config.MaxTiltAngle,
                -_fishEntity.Config.MaxTiltAngle,
                _fishEntity.Config.MaxTiltAngle);

            var tiltDirection = -Mathf.Sign(_visualTransform.localScale.x);
            var targetRotation = Quaternion.Euler(0, 0, targetAngleZ * tiltDirection);

            _visualTransform.localRotation = Quaternion.Slerp(
                _visualTransform.localRotation,
                targetRotation,
                _fishEntity.Config.SteerSpeed * Time.deltaTime
            );

            var isFacingRight = _visualTransform.localScale.x < 0;

            _bubbleParticles.transform.localRotation = Quaternion.Euler(0, isFacingRight ? 180f : 0f, 0);
        }
    }
}
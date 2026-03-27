using System;
using DG.Tweening;
using UnityEngine;

namespace Core.Fish.Modules.Visual
{
    public class FishVisual
    {
        private static readonly int _grayscaleAmount = Shader.PropertyToID("_GrayscaleAmount");

        private readonly FishEntity _fishEntity;
        private readonly Transform _visualTransform;
        private readonly SpriteRenderer _spriteRenderer;

        private Sequence _sequence;

        public FishVisual(FishEntity fishEntity, Transform visualTransform, SpriteRenderer spriteRenderer)
        {
            _fishEntity = fishEntity;
            _visualTransform = visualTransform;
            _spriteRenderer = spriteRenderer;
        }

        public void SetDeadVisuals()
        {
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
            _sequence?.Complete();
            _sequence?.Kill();

            _sequence = DOTween.Sequence()
                .Append(_visualTransform.DOLocalMoveY(_visualTransform.localPosition.y + 1f, 0.5f))
                .Join(_spriteRenderer.DOFade(0f, 0.5f))
                .OnComplete(() => onComplete?.Invoke());

            _sequence.Play();
        }

        public void ResetVisuals()
        {
            _sequence?.Kill();
            _sequence = null;

            _visualTransform.localRotation = Quaternion.identity;
            _visualTransform.localPosition = Vector3.zero;

            _spriteRenderer.color = Color.white;
            _spriteRenderer.SetPropertyBlock(null);
        }

        public void SetSprite(Sprite sprite)
        {
            _spriteRenderer.sprite = sprite;
        }

        public void UpdateVisuals()
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

            var tiltDirection = -_visualTransform.localScale.x;
            var targetRotation = Quaternion.Euler(0, 0, targetAngleZ * tiltDirection);

            _visualTransform.localRotation = Quaternion.Slerp(
                _visualTransform.localRotation,
                targetRotation,
                _fishEntity.Config.SteerSpeed * Time.fixedDeltaTime
            );
        }
    }
}
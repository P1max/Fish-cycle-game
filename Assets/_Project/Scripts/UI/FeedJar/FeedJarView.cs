using System;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace UI.FeedJar
{
    public class FeedJarView : MonoBehaviour
    {
        [SerializeField] private Image _jar;
        [SerializeField] private Button _button;
        [SerializeField] private RectTransform _rect;
        [Title("Animation")]
        [SerializeField] private AnimationCurve _ease;
        [SerializeField] private Color _animColor;

        private event Action _onCLick;
        private Sequence _shakeSeq;

        private void Awake()
        {
            _button.onClick.AddListener(OnClick);
        }

        public void OnClick()
        {
            if (!isActiveAndEnabled) return;

            _onCLick?.Invoke();
        }

        public void PlayShakeAnimation()
        {
            _shakeSeq?.Kill(true);

            _shakeSeq = DOTween.Sequence()
                .Append(_rect.DOPunchAnchorPos(new Vector2(16f, 0), 0.7f, vibrato: 10, elasticity: 1f))
                .Join(_jar.DOColor(_animColor, 0.20f).SetLoops(2, LoopType.Yoyo).SetEase(_ease));
        }

        public void SetPercentOfReadiness(float percentOfReadiness)
        {
            if (isActiveAndEnabled)
            {
            }
        }

        public void Init(Action onCLick)
        {
            _onCLick = onCLick;
        }
    }
}
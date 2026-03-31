using System;
using DG.Tweening;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.FeedJar
{
    public class FeedJarView : BaseView
    {
        [SerializeField] private Image _jar;
        [SerializeField] private Button _button;
        [SerializeField] private RectTransform _rect;

        [Title("Progress Bar")]
        [SerializeField] private Image _progressBar;
        [Tooltip("Границы заполнения бара (X = старт, Y = конец)")]
        [MinMaxSlider(0f, 1f, true)]
        [SerializeField] private Vector2 _fillRange = new(0.15f, 0.85f);

        [Title("Timer Text")]
        [SerializeField] private TextMeshProUGUI _timerText;
        [SerializeField] private string _readyText = "Feed!";

        [Title("Animation Shake (Color)")]
        [SerializeField] private Color _animColor = Color.red;
        [SerializeField] private float _colorInDuration = 0.10f;
        [SerializeField] private Ease _colorInEase = Ease.InCubic;
        [SerializeField] private float _colorOutDuration = 0.15f;
        [SerializeField] private Ease _colorOutEase = Ease.OutSine;

        [Title("Animation Ready Breathing")]
        [SerializeField] private float _breathingScaleMultyplier = 1.05f;
        [SerializeField] private float _breathingDuration = 1f;

        private FeederConfig _config;
        private Action _onCLick;
        private Sequence _shakeSeq;
        private Sequence _readySeq;
        private Vector3 _originalScale;
        private Color _originalColor;
        private bool _isReadyState;

        private void PlayReadyAnimation()
        {
            if (!_isInit) return;

            StopReadyAnimation();

            _readySeq = DOTween.Sequence()
                .Append(_rect.DOPunchScale(_originalScale * 0.10f, 0.7f, vibrato: 0, elasticity: 0f).SetEase(Ease.InSine))
                .AppendCallback(() =>
                {
                    _rect.DOScale(_originalScale * _breathingScaleMultyplier, _breathingDuration)
                        .SetEase(Ease.InOutSine)
                        .SetLoops(-1, LoopType.Yoyo);
                });
        }

        private void StopReadyAnimation()
        {
            if (!_isInit) return;

            _readySeq?.Kill();

            _rect.DOKill();

            _rect.localScale = _originalScale;
        }

        public void OnFeedUsed()
        {
            if (!_isInit) return;

            _isReadyState = false;

            StopReadyAnimation();
        }

        public void PlayShakeAnimation()
        {
            if (!_isInit) return;

            _shakeSeq?.Kill(true);

            _jar.color = _originalColor;

            _shakeSeq = DOTween.Sequence()
                .Insert(0, _rect.DOPunchAnchorPos(new Vector2(16f, 0), 0.7f, vibrato: 10, elasticity: 1f))
                .Insert(0, _jar.DOColor(_animColor, _colorInDuration).SetEase(_colorInEase))
                .Insert(_colorInDuration, _jar.DOColor(_originalColor, _colorOutDuration).SetEase(_colorOutEase));
        }

        public void SetPercentOfReadiness(float percentOfReadiness, float currentCooldown)
        {
            if (!_isInit) return;

            _progressBar.fillAmount = Mathf.Lerp(_fillRange.x, _fillRange.y, percentOfReadiness);

            var currentlyReady = percentOfReadiness >= 1f;

            if (currentlyReady)
            {
                _timerText.text = _readyText;

                if (!_isReadyState)
                {
                    _isReadyState = true;
                    PlayReadyAnimation();
                }
            }
            else
            {
                var remainingTime = currentCooldown * (1f - percentOfReadiness);

                _timerText.text = Mathf.CeilToInt(remainingTime) + "s";

                if (_isReadyState) _isReadyState = false;
            }
        }

        public void Init(Action onClick)
        {
            if (_isInit) return;

            _onCLick = onClick;

            _originalScale = _rect.localScale;
            _originalColor = _jar.color;

            _button.onClick.AddListener(() => _onCLick?.Invoke());

            _isInit = true;
        }
    }
}
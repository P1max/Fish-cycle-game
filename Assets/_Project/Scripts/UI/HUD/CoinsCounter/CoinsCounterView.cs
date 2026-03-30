using DG.Tweening;
using TMPro;
using UnityEngine;

namespace UI.MoneyCounter
{
    public class CoinsCounterView : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _moneyCounterText;
        
        private Color _originalColor;
        private Vector3 _originalScale;
        private Sequence _animationSequence;
        
        private int _currentDisplayedValue;
        
        private void Awake()
        {
            _originalColor = _moneyCounterText.color;
            _originalScale = _moneyCounterText.transform.localScale;
        }
        
        public void SetCurrentCoinsCount(int targetValue, bool instant = false)
        {
            if (instant)
            {
                _animationSequence?.Kill();
                _currentDisplayedValue = targetValue;
                _moneyCounterText.text = _currentDisplayedValue.ToString();
                _moneyCounterText.transform.localScale = _originalScale;
                return;
            }

            if (_currentDisplayedValue == targetValue) return;

            _animationSequence?.Kill();
            
            _moneyCounterText.transform.localScale = _originalScale;
            _moneyCounterText.color = _originalColor;

            _animationSequence = DOTween.Sequence()
                .Append(DOTween.To(() => _currentDisplayedValue, x =>
                {
                    _currentDisplayedValue = x;
                    _moneyCounterText.text = _currentDisplayedValue.ToString();
                }, targetValue, 0.6f).SetEase(Ease.OutQuad)); 
        }
        
        public void PlayNotEnoughMoneyAnimation()
        {
            _animationSequence?.Kill();

            _moneyCounterText.color = _originalColor;
            _moneyCounterText.transform.localScale = _originalScale;

            _animationSequence = DOTween.Sequence()
                .Append(_moneyCounterText.transform.DOScale(_originalScale * 1.2f, 0.15f).SetEase(Ease.OutQuad))
                .Join(_moneyCounterText.DOColor(Color.red, 0.15f))
                .Append(_moneyCounterText.transform.DOScale(_originalScale, 0.15f).SetEase(Ease.InQuad))
                .Join(_moneyCounterText.DOColor(_originalColor, 0.15f));
        }
    }
}
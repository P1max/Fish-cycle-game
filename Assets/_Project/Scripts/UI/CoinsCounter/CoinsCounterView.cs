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
        
        private void Awake()
        {
            _originalColor = _moneyCounterText.color;
            _originalScale = _moneyCounterText.transform.localScale;
        }
        
        public void SetCurrentCoinsCount(int currentFishesCount)
        {
            _moneyCounterText.text = $"{currentFishesCount}";
        }
        
        public void PlayNotEnoughMoneyAnimation()
        {
            _animationSequence?.Kill();

            _moneyCounterText.color = _originalColor;
            _moneyCounterText.transform.localScale = _originalScale;

            _animationSequence = DOTween.Sequence()
                .Append(_moneyCounterText.transform.DOScale(_originalScale * 1.3f, 0.15f).SetEase(Ease.OutQuad))
                .Join(_moneyCounterText.DOColor(Color.red, 0.15f))
                .Append(_moneyCounterText.transform.DOScale(_originalScale, 0.15f).SetEase(Ease.InQuad))
                .Join(_moneyCounterText.DOColor(_originalColor, 0.15f));
        }
    }
}
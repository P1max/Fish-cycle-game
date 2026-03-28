using DG.Tweening;
using TMPro;
using UnityEngine;

namespace UI
{
    public class FishesCounterView : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _fishCountText;

        private Color _originalColor;
        private Vector3 _originalScale;
        private Sequence _animationSequence;

        private void Awake()
        {
            _originalColor = _fishCountText.color;
            _originalScale = _fishCountText.transform.localScale;
        }
        
        public void SetCurrentFishesCount(int currentFishesCount, int maxFishesCount)
        {
            _fishCountText.text = $"{currentFishesCount}/{maxFishesCount}";
        }

        public void PlayLimitReachedAnimation()
        {
            _animationSequence?.Kill();

            _fishCountText.color = _originalColor;
            _fishCountText.transform.localScale = _originalScale;

            _animationSequence = DOTween.Sequence()
                .Append(_fishCountText.transform.DOScale(_originalScale * 1.3f, 0.15f).SetEase(Ease.OutQuad))
                .Join(_fishCountText.DOColor(Color.red, 0.15f))
                .Append(_fishCountText.transform.DOScale(_originalScale, 0.15f).SetEase(Ease.InQuad))
                .Join(_fishCountText.DOColor(_originalColor, 0.15f));
        }
    }
}
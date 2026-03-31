using DG.Tweening;
using UnityEngine;

namespace UI.Background
{
    public class BackgroundView : BaseView
    {
        [SerializeField] private RectTransform _backgroundRect;
        [SerializeField] private float _animationDuration = 1.5f;

        public void SetScale(float targetScale, bool animate)
        {
            if (!_isInit) return;

            _backgroundRect.DOKill();

            if (animate)
            {
                _backgroundRect.DOScale(targetScale, _animationDuration).SetEase(Ease.InOutSine);
            }
            else
            {
                _backgroundRect.localScale = Vector3.one * targetScale;
            }
        }

        public void Init()
        {
            _isInit = true;
        }
    }
}
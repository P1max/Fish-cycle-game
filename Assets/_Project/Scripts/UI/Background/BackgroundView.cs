using DG.Tweening;
using UnityEngine;

namespace UI.Background
{
    public class BackgroundView : MonoBehaviour
    {
        [SerializeField] private RectTransform _backgroundRect;
        [SerializeField] private float _animationDuration = 1.5f;

        public void SetScale(float targetScale, bool animate)
        {
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
    }
}
using UnityEngine;
using TMPro;
using DG.Tweening;

namespace Core.Fish.Modules.Visual
{
    public class FishIndicator
    {
        private readonly GameObject _bubbleContainer;
        private readonly SpriteRenderer _iconRenderer;
        private readonly TextMeshPro _timerText;
        private readonly Sprite _hungerSprite;
        private readonly Sprite _deathSprite;
        private readonly FishEntity _fish;
        private readonly Vector3 _originalScale;

        private bool _isShowingDeath;
        private Sequence _popSequence;

        public FishIndicator(FishEntity fish, GameObject bubbleContainer, SpriteRenderer iconRenderer,
            TextMeshPro timerText, Sprite hungerSprite, Sprite deathSprite)
        {
            _fish = fish;
            _bubbleContainer = bubbleContainer;
            _iconRenderer = iconRenderer;
            _timerText = timerText;
            _hungerSprite = hungerSprite;
            _deathSprite = deathSprite;

            _originalScale = _bubbleContainer.transform.localScale;
            _bubbleContainer.transform.localScale = Vector3.zero;
            _bubbleContainer.SetActive(false);
        }

        private void ShowHunger()
        {
            if (_isShowingDeath) return;

            _iconRenderer.sprite = _hungerSprite;
            _timerText.text = "";

            PopUp();
        }

        private void PopUp()
        {
            if (_bubbleContainer.activeSelf) return;

            _bubbleContainer.SetActive(true);
            _bubbleContainer.transform.localScale = Vector3.zero;

            _popSequence?.Kill();

            _popSequence = DOTween.Sequence()
                .Append(_bubbleContainer.transform.DOScale(_originalScale, 0.4f).SetEase(Ease.OutBack));
        }

        private void ShowDeathTimer(float remainingSeconds)
        {
            if (!_isShowingDeath)
            {
                _isShowingDeath = true;
                _iconRenderer.sprite = _deathSprite;

                PopUp();
            }

            _timerText.text = Mathf.CeilToInt(remainingSeconds).ToString();
            _timerText.color = remainingSeconds <= 3f ? Color.red : Color.white;
        }

        public void Hide()
        {
            if (!_bubbleContainer.activeSelf) return;

            _isShowingDeath = false;

            _popSequence?.Kill();

            _bubbleContainer.transform.DOScale(Vector3.zero, 0.2f)
                .SetEase(Ease.InBack)
                .OnComplete(() => _bubbleContainer.SetActive(false));
        }

        public void Tick()
        {
            if (_fish.LifeCycle.TimeToDeath <= _fish.CommonFishConfig.DeathTimerThreshold)
            {
                ShowDeathTimer(_fish.LifeCycle.TimeToDeath);
            }
            else if (_fish.Hunger.CurrentHungerPercent >= _fish.CommonFishConfig.HungerIndicatorThreshold)
            {
                ShowHunger();
            }
            else
            {
                Hide();
            }
        }
    }
}
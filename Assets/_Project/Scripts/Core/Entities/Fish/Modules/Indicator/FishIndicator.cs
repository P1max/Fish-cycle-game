using UnityEngine;
using TMPro;
using DG.Tweening;

namespace Core.Fish.Modules.Visual
{
    public class FishIndicator
    {
        private readonly GameObject _hungryContainer;
        private readonly GameObject _deathContainer;
        private readonly TextMeshPro _timerText;
        private readonly FishEntity _fish;
        private readonly Vector3 _originalHungryConScale;
        private readonly Vector3 _originalDeathConScale;

        private Sequence _popHungrySequence;
        private Sequence _popDeathSequence;

        public FishIndicator(FishEntity fish, GameObject hungryContainer, GameObject deathContainer, TextMeshPro timerText)
        {
            _fish = fish;
            _hungryContainer = hungryContainer;
            _deathContainer = deathContainer;
            _timerText = timerText;

            _originalHungryConScale = _hungryContainer.transform.localScale;
            _originalDeathConScale = _deathContainer.transform.localScale;

            _hungryContainer.transform.localScale = Vector3.zero;
            _deathContainer.transform.localScale = Vector3.zero;

            _hungryContainer.SetActive(false);
            _deathContainer.SetActive(false);
        }

        private void ShowHunger() => PopUpHungry();

        private void ShowDeathTimer(float remainingSeconds)
        {
            _timerText.text = Mathf.CeilToInt(remainingSeconds).ToString();

            PopUpDeath();
        }

        private void PopUpHungry()
        {
            if (_hungryContainer.activeSelf) return;

            _hungryContainer.SetActive(true);
            _hungryContainer.transform.localScale = Vector3.zero;

            _popHungrySequence?.Kill();

            _popHungrySequence = DOTween.Sequence()
                .Append(_hungryContainer.transform.DOScale(_originalHungryConScale, 0.4f).SetEase(Ease.OutBack));
        }

        private void PopUpDeath()
        {
            if (_deathContainer.activeSelf) return;

            _deathContainer.SetActive(true);
            _deathContainer.transform.localScale = Vector3.zero;

            _popDeathSequence?.Kill();

            _popDeathSequence = DOTween.Sequence()
                .Append(_deathContainer.transform.DOScale(_originalDeathConScale, 0.4f).SetEase(Ease.OutBack));
        }

        public void HideDeath()
        {
            if (!_deathContainer.activeSelf) return;

            _popDeathSequence?.Kill();

            _deathContainer.transform.DOScale(Vector3.zero, 0.2f)
                .SetEase(Ease.InBack)
                .OnComplete(() => _deathContainer.SetActive(false));
        }

        public void HideHungry()
        {
            if (!_hungryContainer.activeSelf) return;

            _popHungrySequence?.Kill();

            _hungryContainer.transform.DOScale(Vector3.zero, 0.2f)
                .SetEase(Ease.InBack)
                .OnComplete(() => _hungryContainer.SetActive(false));
        }

        public void Tick()
        {
            if (_fish.LifeCycle.TimeToDeath <= _fish.CommonFishConfig.DeathTimerThreshold)
            {
                ShowDeathTimer(_fish.LifeCycle.TimeToDeath);
            }
            else
            {
                HideDeath();
            }

            if (_fish.Hunger.CurrentHungerPercent >= _fish.CommonFishConfig.HungerIndicatorThreshold)
            {
                ShowHunger();
            }
            else
            {
                HideHungry();
            }
        }
    }
}
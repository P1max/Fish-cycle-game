using System;
using UnityEngine;
using UnityEngine.UI;

namespace UI.FeedJar
{
    public class FeedJarView : MonoBehaviour
    {
        [SerializeField] private Button _button;

        private event Action _onCLick;

        private void Awake()
        {
            _button.onClick.AddListener(OnClick);
        }

        public void OnClick()
        {
            if (!isActiveAndEnabled) return;

            _onCLick?.Invoke();
        }

        public void Init(Action onCLick)
        {
            _onCLick = onCLick;
        }

        public void PlayShakeAnimation()
        {
            
        }

        public void SetPercentOfReadiness(float percentOfReadiness)
        {
            if (isActiveAndEnabled)
            {
            }
        }
    }
}
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Conveyor
{
    public class FishItemView : MonoBehaviour
    {
        [SerializeField] private Image _fishSprite;
        [SerializeField] private TextMeshProUGUI _lifeTimeText;
        [SerializeField] private TextMeshProUGUI _incomeText;
        [SerializeField] private TextMeshProUGUI _fishPrice;
        [SerializeField] private Button _button;
        [SerializeField] private RectTransform _rect;

        private Action _buyAction;
        
        public RectTransform Rect => _rect;

        private void Awake()
        {
            _button.onClick.AddListener(() => _buyAction?.Invoke());
        }

        public void PlayShakeAnimation()
        {
            
        }

        public void SetButtonAction(Action action)
        {
            _buyAction = action;
        }

        public void SetData(Sprite sprite, float lifeTime, float income, float fishPrice)
        {
            _fishSprite.sprite = sprite;
            _lifeTimeText.text = lifeTime.ToString("F2");
            _incomeText.text = income.ToString("F2");
            _fishPrice.text = fishPrice.ToString("F2");
        }
    }
}
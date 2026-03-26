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
        [SerializeField] private TextMeshProUGUI _purchasedText;

        private Action<bool> _buyAction;
        private bool _purchased;

        public RectTransform Rect => _rect;

        private void Awake()
        {
            _button.onClick.AddListener(() => _buyAction?.Invoke(_purchased));
        }

        public void PlayShakeAnimation()
        {
        }

        public void SetPurchasedState()
        {
            _purchased = true;
            
            _fishSprite.color = new Color32(255, 255, 255, 0);

            _purchasedText.gameObject.SetActive(true);
        }

        public void SetButtonAction(Action<bool> action)
        {
            _buyAction = action;
        }

        public void SetData(Sprite sprite, float lifeTime, int income, int fishPrice)
        {
            _fishSprite.sprite = sprite;
            _fishSprite.color = new Color32(255, 255, 255, 255);
            _lifeTimeText.text = $"{lifeTime} s";
            _incomeText.text = $"{income} $/s";
            _fishPrice.text = fishPrice.ToString();


            _purchased = false;
            _purchasedText.gameObject.SetActive(false);
        }
    }
}
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

        [SerializeField] private GameObject _coin;
        [SerializeField] private Color _defaultItemPriceColor;
        [SerializeField] private Color _freeItemColor;
        

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

        public void SetData(Sprite sprite, int lifeTime, int income, int fishPrice)
        {
            _fishSprite.sprite = sprite;
            _fishSprite.color = new Color32(255, 255, 255, 255);
            _lifeTimeText.text = $"{lifeTime} s";
            _incomeText.text = $"{income} $/s";

            if (fishPrice <= 0)
            {
                _fishPrice.text = "Free";
                _fishPrice.color = _freeItemColor;
                _coin.SetActive(false);
            }
            else
            {
                _fishPrice.text = $"{fishPrice}";
                _fishPrice.color = _defaultItemPriceColor;
                _coin.SetActive(true);
            }
            
            _purchased = false;
            _purchasedText.gameObject.SetActive(false);
        }
    }
}
using TMPro;
using UnityEngine;

namespace UI.MoneyCounter
{
    public class CoinsCounterView : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _moneyCounterText;
        
        public void SetCurrentCoinsCount(int currentFishesCount)
        {
            _moneyCounterText.text = $"{currentFishesCount}";
        }
    }
}
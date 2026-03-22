using TMPro;
using UnityEngine;

namespace UI
{
    public class FishesCounterView : MonoBehaviour
    {
        private int _maxFishesCounter;

        [SerializeField] private TextMeshProUGUI _fishCountText;

        public void Init(int maxFishesCounter) => _maxFishesCounter = maxFishesCounter;

        public void SetCurrentFishesCount(int currentFishesCount)
        {
            _fishCountText.text = $"{currentFishesCount}/{_maxFishesCounter}";
        }
    }
}
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Project.UI.AquariumUpgrader
{
    public class AquariumUpgraderView : MonoBehaviour
    {
        [SerializeField] private Button _upgradeButton;
        [SerializeField] private TextMeshProUGUI _upgradeCost;

        public event Action OnButtonClicked;

        private void Awake()
        {
            _upgradeButton.onClick.AddListener(() => OnButtonClicked?.Invoke());
        }

        public void SetUpgradeCost(int cost)
        {
            _upgradeCost.text = cost == int.MaxValue ? "Max" : cost.ToString();
        }
    }
}
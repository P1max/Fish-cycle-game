using System;
using TMPro;
using UI;
using UnityEngine;
using UnityEngine.UI;

namespace _Project.UI.AquariumUpgrader
{
    public class AquariumUpgraderView : BaseView
    {
        [SerializeField] private Button _upgradeButton;
        [SerializeField] private TextMeshProUGUI _upgradeCost;

        private event Action _onClick;

        public void SetUpgradeCost(int cost)
        {
            if (!_isInit) return;

            _upgradeCost.text = cost == int.MaxValue ? "Max" : cost.ToString();
        }

        public void Init(Action onClick)
        {
            _onClick = onClick;

            _upgradeButton.onClick.AddListener(() => _onClick?.Invoke());

            _isInit = true;
        }
    }
}
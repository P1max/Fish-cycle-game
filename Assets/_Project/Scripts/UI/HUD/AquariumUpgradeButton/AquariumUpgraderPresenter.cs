using System;
using _Project.Core.Interfaces;
using Core.Game;
using Core.Game.Upgrade;
using UI.MoneyCounter;

namespace _Project.UI.AquariumUpgrader
{
    public class AquariumUpgraderPresenter : IUIInit, IDisposable
    {
        private readonly AquariumUpgraderView _view;
        private readonly UpgradeManager _upgradeManager;
        private readonly BalanceManager _balanceManager;
        private readonly CoinsCounterPresenter _coinsCounterPresenter;


        public AquariumUpgraderPresenter(AquariumUpgraderView view, UpgradeManager upgradeManager, BalanceManager balanceManager,
            CoinsCounterPresenter coinsCounterPresenter)
        {
            _view = view;
            _upgradeManager = upgradeManager;
            _balanceManager = balanceManager;
            _coinsCounterPresenter = coinsCounterPresenter;
        }

        private void HandleUpgrade(UpgradesConfig.LevelData upgradeData) => _view.SetUpgradeCost(_upgradeManager.NextLevelCost);

        public void Init()
        {
            _upgradeManager.OnAquariumUpgrade += HandleUpgrade;

            _view.Init(() =>
            {
                if (!_upgradeManager.IsMaxLevel)
                {
                    if (_balanceManager.TrySpendCoins(_upgradeManager.NextLevelCost))
                        _upgradeManager.UpgradeLevel();
                    else _coinsCounterPresenter.PlayNotEnoughMoneyAnimation();
                }
            });
            _view.SetUpgradeCost(_upgradeManager.NextLevelCost);
        }

        public void Dispose()
        {
            _upgradeManager.OnAquariumUpgrade -= HandleUpgrade;
        }
    }
}
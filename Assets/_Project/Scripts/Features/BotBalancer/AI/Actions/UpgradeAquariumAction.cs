using Core.Game;
using Core.Game.Upgrade;
using Features.BotBalancer.AI.Configs;

namespace Features.BotBalancer.AI
{
    public class UpgradeAquariumAction : IBotAction
    {
        private readonly UpgradeManager _upgradeManager;
        private readonly BalanceManager _balanceManager;
        private readonly FishesManager _fishesManager;
        private readonly UpgradeAquariumActionConfig _config;

        public UpgradeAquariumAction(
            UpgradeManager upgradeManager,
            BalanceManager balanceManager,
            FishesManager fishesManager,
            UpgradeAquariumActionConfig config)
        {
            _upgradeManager = upgradeManager;
            _balanceManager = balanceManager;
            _fishesManager = fishesManager;
            _config = config;
        }

        public float Evaluate()
        {
            if (_upgradeManager.IsMaxLevel) return 0f;
            if (_balanceManager.CurrentCoinsCount < _upgradeManager.NextLevelCost) return 0f;

            var crowdedness = (float)_fishesManager.CurrentFishCount / _fishesManager.MaxFishesCount;

            if (crowdedness < _config.MinCrowdedThreshold)
                return 0f;

            return _config.BaseWeight;
        }

        public void Execute()
        {
            if (_balanceManager.TrySpendCoins(_upgradeManager.NextLevelCost))
                _upgradeManager.UpgradeLevel();
        }
    }
}
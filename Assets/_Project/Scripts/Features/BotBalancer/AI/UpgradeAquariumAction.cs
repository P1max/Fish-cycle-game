using Core.Game;
using Core.Game.Upgrade;

namespace Features.BotBalancer.AI
{
    public class UpgradeAquariumAction : IBotAction
    {
        private readonly UpgradeManager _upgradeManager;
        private readonly BalanceManager _balanceManager;
        private readonly BotProfileConfig _profile;

        public UpgradeAquariumAction(
            UpgradeManager upgradeManager,
            BalanceManager balanceManager,
            BotProfileConfig profile)
        {
            _upgradeManager = upgradeManager;
            _balanceManager = balanceManager;
            _profile = profile;
        }

        public float Evaluate()
        {
            if (_upgradeManager.IsMaxLevel) return 0f;

            if (_balanceManager.CurrentCoinsCount < _upgradeManager.NextLevelCost) return 0f;

            return _profile.UpgradeAquariumWeight;
        }

        public void Execute()
        {
            if (_balanceManager.TrySpendCoins(_upgradeManager.NextLevelCost))
                _upgradeManager.UpgradeLevel();
        }
    }
}
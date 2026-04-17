using Core.Game;
using Core.Game.Upgrade;
using Features.BotBalancer.AI.Configs;
using Spawners;
using UnityEngine;

namespace Features.BotBalancer.AI
{
    public class CollectCoinsAction : IBotAction
    {
        private readonly CoinsPool _coinsPool;
        private readonly CollectCoinsActionConfig _config;
        private readonly BalanceManager _balanceManager;
        private readonly StoreManager _storeManager;
        private readonly UpgradeManager _upgradeManager;

        public CollectCoinsAction(
            CoinsPool coinsPool,
            CollectCoinsActionConfig config,
            BalanceManager balanceManager,
            StoreManager storeManager,
            UpgradeManager upgradeManager)
        {
            _coinsPool = coinsPool;
            _config = config;
            _balanceManager = balanceManager;
            _storeManager = storeManager;
            _upgradeManager = upgradeManager;
        }

        public float Evaluate()
        {
            var coinsCount = _coinsPool.ActiveCoins.Count;

            if (coinsCount == 0) return 0f;

            var fillRatio = Mathf.Clamp01((float)coinsCount / _config.MaxCoinsThreshold);

            var curveMultiplier = _config.CoinWeightCurve.Evaluate(fillRatio);
            var currentWeight = _config.BaseWeight * curveMultiplier;

            if (!_config.UseDesperationStrategy)
            {
                return Mathf.Max(currentWeight, 0.05f);
            }

            var floatingWealth = coinsCount;
            var needsMoneyBadly = false;

            if (!_upgradeManager.IsMaxLevel)
            {
                var missingForUpgrade = _upgradeManager.NextLevelCost - _balanceManager.CurrentCoinsCount;

                if (missingForUpgrade > 0 && missingForUpgrade <= floatingWealth)
                    needsMoneyBadly = true;
            }

            if (!needsMoneyBadly)
            {
                StoreLot bestLot = null;
                var highestRoi = -1f;

                foreach (var lot in _storeManager.AvailableLots)
                {
                    if (lot.IsPurchased || !lot.IsVisible) continue;

                    var baseConfig = _storeManager.GetFishConfig(lot.FishId);
                    var roi = lot.GetRoi(baseConfig);

                    if (roi > highestRoi)
                    {
                        highestRoi = roi;
                        bestLot = lot;
                    }
                }
                
                if (bestLot != null)
                {
                    var missingForDreamFish = bestLot.Price - _balanceManager.CurrentCoinsCount;

                    if (missingForDreamFish > 0 && missingForDreamFish <= floatingWealth)
                    {
                        needsMoneyBadly = true;
                    }
                }
            }

            if (needsMoneyBadly)
            {
                currentWeight = _config.BaseWeight * _config.DesperationMultiplier;
            }

            return Mathf.Max(currentWeight, 0.05f);
        }

        public void Execute()
        {
            var activeCoins = _coinsPool.ActiveCoins;

            for (var i = activeCoins.Count - 1; i >= 0; i--)
            {
                var coin = activeCoins[i];

                coin.Collect();
            }
        }
    }
}
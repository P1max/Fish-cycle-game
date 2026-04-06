using Core.Game;
using Features.BotBalancer.AI.Configs;
using Spawners;

namespace Features.BotBalancer.AI
{
    public class CollectDeadFishAction : IBotAction
    {
        private readonly FishPool _fishPool;
        private readonly CleanDeadFishActionConfig _config;
        private readonly StoreManager _storeManager;
        private readonly FishesManager _aquarium;
        private readonly BalanceManager _balanceManager;

        private bool _isAirlockMode;

        public CollectDeadFishAction(
            FishPool fishPool,
            CleanDeadFishActionConfig config,
            StoreManager storeManager,
            FishesManager aquarium,
            BalanceManager balanceManager)
        {
            _fishPool = fishPool;
            _config = config;
            _storeManager = storeManager;
            _aquarium = aquarium;
            _balanceManager = balanceManager;
        }

        public float Evaluate()
        {
            _isAirlockMode = false;
            var deadCount = 0;
            var aliveCount = 0;
            var worstAliveFishRoi = float.MaxValue;

            foreach (var fish in _fishPool.ActiveFishes)
            {
                if (!fish.IsAlive)
                {
                    deadCount++;
                }
                else
                {
                    aliveCount++;

                    var currentFishRoi = fish.Config.GetRoi();

                    if (currentFishRoi < worstAliveFishRoi)
                    {
                        worstAliveFishRoi = currentFishRoi;
                    }
                }
            }

            if (deadCount == 0) return 0f;
            if (aliveCount == 0) return _config.BaseWeight;

            if (!_aquarium.CanAddFish)
            {
                if (!_config.UseAirlockStrategy) return _config.BaseWeight;

                var canBuyBetterFish = false;

                foreach (var lot in _storeManager.AvailableLots)
                {
                    if (lot.IsPurchased || !lot.IsVisible || lot.Price > _balanceManager.CurrentCoinsCount) continue;

                    var baseConfig = _storeManager.GetFishConfig(lot.FishId);

                    var actualIncome = baseConfig.IncomeCoins * lot.Quality;
                    var actualLifetime = baseConfig.LifetimeSeconds * lot.Quality;
                    var actualPrice = lot.Price == 0 ? 1 : lot.Price;

                    var storeFishRoi = (actualIncome * (actualLifetime / baseConfig.IncomeCooldownSeconds)) / actualPrice;

                    if (storeFishRoi > worstAliveFishRoi * _config.UpgradeThreshold)
                    {
                        canBuyBetterFish = true;

                        break;
                    }
                }

                if (canBuyBetterFish)
                {
                    _isAirlockMode = true;

                    return _config.BaseWeight;
                }

                return _config.BaseWeight * _config.AirlockWeightMultiplier;
            }

            return _config.BaseWeight;
        }

        public void Execute()
        {
            var fishes = _fishPool.ActiveFishes;

            for (var i = fishes.Count - 1; i >= 0; i--)
            {
                var fish = fishes[i];

                if (!fish.IsAlive)
                {
                    fish.Collect();

                    if (_isAirlockMode) break;
                }
            }
        }
    }
}
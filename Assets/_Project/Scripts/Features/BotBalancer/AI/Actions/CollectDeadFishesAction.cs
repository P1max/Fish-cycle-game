using Core.Game;
using Spawners;

namespace Features.BotBalancer.AI
{
    public class CollectDeadFishAction : IBotAction
    {
        private readonly FishPool _fishPool;
        private readonly BotProfileConfig _profile;
        private readonly StoreManager _storeManager;
        private readonly FishesManager _aquarium;
        private readonly BalanceManager _balanceManager;

        private bool _isAirlockMode;

        public CollectDeadFishAction(
            FishPool fishPool,
            BotProfileConfig profile,
            StoreManager storeManager,
            FishesManager aquarium,
            BalanceManager balanceManager)
        {
            _fishPool = fishPool;
            _profile = profile;
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

                    var expectedTotalIncome = fish.Config.IncomeCoins * (fish.Config.LifetimeSeconds / fish.Config.IncomeCooldownSeconds);
                    var actualPrice = fish.Config.Price == 0 ? 1 : fish.Config.Price;
                    var currentFishRoi = expectedTotalIncome / actualPrice;

                    if (currentFishRoi < worstAliveFishRoi)
                    {
                        worstAliveFishRoi = currentFishRoi;
                    }
                }
            }

            if (deadCount == 0) return 0f;
            if (aliveCount == 0) return _profile.CleanDeadFishWeight;

            if (!_aquarium.CanAddFish)
            {
                if (!_profile.UseAirlockStrategy) return _profile.CleanDeadFishWeight;

                var canBuyBetterFish = false;

                foreach (var lot in _storeManager.AvailableLots)
                {
                    if (lot.IsPurchased || !lot.IsVisible || lot.Price > _balanceManager.CurrentCoinsCount) continue;

                    var baseConfig = _storeManager.GetFishConfig(lot.FishId);

                    var actualIncome = baseConfig.IncomeCoins * lot.Quality;
                    var actualLifetime = baseConfig.LifetimeSeconds * lot.Quality;
                    var actualPrice = lot.Price == 0 ? 1 : lot.Price;

                    var storeFishRoi = (actualIncome * (actualLifetime / baseConfig.IncomeCooldownSeconds)) / actualPrice;

                    if (storeFishRoi > worstAliveFishRoi * _profile.UpgradeThreshold)
                    {
                        canBuyBetterFish = true;

                        break;
                    }
                }

                if (canBuyBetterFish)
                {
                    _isAirlockMode = true;

                    return _profile.CleanDeadFishWeight;
                }

                return _profile.CleanDeadFishWeight * _profile.AirlockWeightMultiplier;
            }

            return _profile.CleanDeadFishWeight;
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
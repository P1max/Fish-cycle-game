using Core.Feed;
using Core.Game;
using Core.Loaders;
using Spawners;
using UnityEngine;

namespace Features.BotBalancer.AI
{
    public class FeedFishesAction : IBotAction
    {
        private readonly FeedManager _feedManager;
        private readonly BotProfileConfig _profile;
        private readonly FishesManager _aquarium;
        private readonly BalanceManager _balanceManager;
        private readonly FishPool _fishPool;
        private readonly CommonFishConfig _commonFishConfig;
        private readonly FishesConfigsLoader _fishesConfigsLoader;

        public FeedFishesAction(
            FeedManager feedManager,
            BotProfileConfig profile,
            FishesManager aquarium,
            BalanceManager balanceManager,
            FishPool fishPool,
            CommonFishConfig commonFishConfig,
            FishesConfigsLoader fishesConfigsLoader)
        {
            _feedManager = feedManager;
            _profile = profile;
            _aquarium = aquarium;
            _balanceManager = balanceManager;
            _fishPool = fishPool;
            _commonFishConfig = commonFishConfig;
            _fishesConfigsLoader = fishesConfigsLoader;
        }

        public float Evaluate()
        {
            if (!_feedManager.IsReady) return 0f;

            var bestPotentialRoi = 0f;

            foreach (var baseConfig in _fishesConfigsLoader.LoadedFishesDict.Values)
            {
                if (baseConfig.Price > _balanceManager.CurrentCoinsCount) continue;

                var expectedTotalIncome = baseConfig.IncomeCoins * (baseConfig.LifetimeSeconds / baseConfig.IncomeCooldownSeconds);
                var actualPrice = baseConfig.Price == 0 ? 1 : baseConfig.Price;
                var potentialRoi = expectedTotalIncome / actualPrice;

                if (potentialRoi > bestPotentialRoi)
                {
                    bestPotentialRoi = potentialRoi;
                }
            }

            var maxHungerElite = 0f;
            var maxHungerTrash = 0f;
            var hasAliveFishes = false;
            var hasTrash = false;

            foreach (var fish in _fishPool.ActiveFishes)
            {
                if (!fish.IsAlive) continue;

                hasAliveFishes = true;

                var expectedTotalIncome = fish.Config.IncomeCoins * (fish.Config.LifetimeSeconds / fish.Config.IncomeCooldownSeconds);
                var actualPrice = fish.Config.Price == 0 ? 1 : fish.Config.Price;
                var currentFishRoi = expectedTotalIncome / actualPrice;

                var isTrash = (currentFishRoi * _profile.UpgradeThreshold) < bestPotentialRoi;

                if (isTrash)
                {
                    hasTrash = true;

                    if (fish.Hunger.CurrentHungerPercent > maxHungerTrash)
                        maxHungerTrash = fish.Hunger.CurrentHungerPercent;
                }
                else
                {
                    if (fish.Hunger.CurrentHungerPercent > maxHungerElite)
                        maxHungerElite = fish.Hunger.CurrentHungerPercent;
                }
            }

            if (!hasAliveFishes) return 0f;

            var hungerThreshold = _commonFishConfig.HungerIndicatorThreshold;

            if (maxHungerElite >= hungerThreshold)
            {
                var panicFactor = (maxHungerElite - hungerThreshold) / (100f - hungerThreshold);

                return _profile.FeedFishesWeight * Mathf.Clamp01(panicFactor);
            }

            if (!_aquarium.CanAddFish && hasTrash && _profile.UseStarvationStrategy)
                return _profile.FeedFishesWeight * _profile.StarvationWeightMultiplier;

            var overallMaxHunger = Mathf.Max(maxHungerElite, maxHungerTrash);

            if (overallMaxHunger < hungerThreshold)
                return 0.05f;

            var normalPanic = (overallMaxHunger - hungerThreshold) / (100f - hungerThreshold);

            return _profile.FeedFishesWeight * Mathf.Clamp01(normalPanic);
        }

        public void Execute()
        {
            _feedManager.TryFeed();
        }
    }
}
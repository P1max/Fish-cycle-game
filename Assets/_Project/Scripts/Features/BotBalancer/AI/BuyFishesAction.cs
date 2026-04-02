using Core.Game;

namespace Features.BotBalancer.AI
{
    public class BuyFishAction : IBotAction
    {
        private readonly StoreManager _storeManager;
        private readonly FishesManager _aquarium;
        private readonly BalanceManager _balanceManager;
        private readonly BotProfileConfig _profile;

        private StoreLot _bestLotToBuy;

        public BuyFishAction(
            StoreManager storeManager,
            FishesManager aquarium,
            BalanceManager balanceManager,
            BotProfileConfig profile)
        {
            _storeManager = storeManager;
            _aquarium = aquarium;
            _balanceManager = balanceManager;
            _profile = profile;
        }

        public float Evaluate()
        {
            _bestLotToBuy = null;

            if (!_aquarium.CanAddFish) return 0f;

            var highestLotScore = 0f;

            foreach (var lot in _storeManager.AvailableLots)
            {
                if (lot.IsPurchased) continue;

                if (!lot.IsVisible) continue;

                if (lot.Price > _balanceManager.CurrentCoinsCount) continue;

                var lotScore = _profile.BuyFishWeight;

                if (lotScore > highestLotScore)
                {
                    highestLotScore = lotScore;
                    _bestLotToBuy = lot;
                }
            }

            return highestLotScore;
        }

        public void Execute()
        {
            if (_bestLotToBuy == null) return;

            _storeManager.TryBuyFish(_bestLotToBuy);
        }
    }
}
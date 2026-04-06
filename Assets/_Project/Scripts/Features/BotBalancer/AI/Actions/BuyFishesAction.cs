using Core.Game;
using Features.BotBalancer.AI.Configs;

namespace Features.BotBalancer.AI
{
    public class BuyFishAction : IBotAction
    {
        private readonly StoreManager _storeManager;
        private readonly FishesManager _aquarium;
        private readonly BalanceManager _balanceManager;
        private readonly BuyFishActionConfig _config;

        private StoreLot _bestLotToBuy;

        public BuyFishAction(
            StoreManager storeManager,
            FishesManager aquarium,
            BalanceManager balanceManager,
            BuyFishActionConfig config)
        {
            _storeManager = storeManager;
            _aquarium = aquarium;
            _balanceManager = balanceManager;
            _config = config;
        }

        public float Evaluate()
        {
            _bestLotToBuy = null;

            if (!_aquarium.CanAddFish) return 0f;

            var highestRoi = -1f;

            foreach (var lot in _storeManager.AvailableLots)
            {
                if (lot.IsPurchased) continue;
                if (!lot.IsVisible) continue;
                if (lot.Price > _balanceManager.CurrentCoinsCount) continue;

                var baseConfig = _storeManager.GetFishConfig(lot.FishId);

                var roi = lot.GetRoi(baseConfig);

                if (roi > highestRoi)
                {
                    highestRoi = roi;
                    _bestLotToBuy = lot;
                }
            }
            
            return _bestLotToBuy != null ? _config.BaseWeight : 0f;
        }

        public void Execute()
        {
            if (_bestLotToBuy == null) return;

            _storeManager.TryBuyFish(_bestLotToBuy);
        }
    }
}
using Spawners;

namespace Features.BotBalancer.AI
{
    public class CollectCoinsAction : IBotAction
    {
        private readonly CoinsPool _coinsPool;
        private readonly BotProfileConfig _profile;

        public CollectCoinsAction(CoinsPool coinsPool, BotProfileConfig profile)
        {
            _coinsPool = coinsPool;
            _profile = profile;
        }

        public float Evaluate()
        {
            if (_coinsPool.ActiveCoins.Count == 0) return 0f;

            return _profile.CollectCoinsWeight;
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
using Spawners;

namespace Features.BotBalancer.AI
{
    public class CollectDeadFishAction : IBotAction
    {
        private readonly FishPool _fishPool;
        private readonly BotProfileConfig _profile;

        public CollectDeadFishAction(FishPool fishPool, BotProfileConfig profile)
        {
            _fishPool = fishPool;
            _profile = profile;
        }

        public float Evaluate()
        {
            var fishes = _fishPool.ActiveFishes;

            foreach (var fish in fishes)
            {
                if (fish.IsAlive) continue;

                return _profile.CleanDeadFishWeight;
            }

            return 0f;
        }

        public void Execute()
        {
            var fishes = _fishPool.ActiveFishes;

            for (var i = fishes.Count - 1; i >= 0; i--)
            {
                var fish = fishes[i];

                if (!fish.IsAlive) fish.Collect();
            }
        }
    }
}
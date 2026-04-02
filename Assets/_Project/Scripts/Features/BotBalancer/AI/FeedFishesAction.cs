using Core.Feed;

namespace Features.BotBalancer.AI
{
    public class FeedFishesAction : IBotAction
    {
        private readonly FeedManager _feedManager;
        private readonly BotProfileConfig _profile;

        public FeedFishesAction(FeedManager feedManager, BotProfileConfig profile)
        {
            _feedManager = feedManager;
            _profile = profile;
        }

        public float Evaluate()
        {
            if (!_feedManager.IsReady)
            {
                return 0f;
            }

            return _profile.FeedFishesWeight;
        }

        public void Execute()
        {
            _feedManager.TryFeed();
        }
    }
}
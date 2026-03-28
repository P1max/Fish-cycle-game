using Core.Feed;

namespace UI.FeedJar
{
    public class FeedJarPresenter
    {
        private readonly FeedJarView _feedJarView;
        private readonly FeedManager _feedManager;

        public FeedJarPresenter(FeedJarView feedJarView, FeedManager feedManager)
        {
            _feedJarView = feedJarView;
            _feedManager = feedManager;

            _feedManager.OnNormalizedTime += ChangeFeedJarPercentage;

            _feedJarView.Init(() =>
            {
                if (_feedManager.TryFeed())
                {
                    _feedJarView.OnFeedUsed();
                }
                else
                {
                    _feedJarView.PlayShakeAnimation();
                }
            });
        }

        private void ChangeFeedJarPercentage(float percentage)
        {
            _feedJarView.SetPercentOfReadiness(percentage, _feedManager.ActiveCooldown);
        }
    }
}
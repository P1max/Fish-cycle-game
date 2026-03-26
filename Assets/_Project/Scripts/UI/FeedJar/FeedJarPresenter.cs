using Core.Feed;

namespace UI.FeedJar
{
    public class FeedJarPresenter
    {
        private readonly FeedJarView _feedJarView;
        private readonly FeedManager _feedManager;

        public FeedJarPresenter(FeedJarView feedJarView, FeedManager feedManager, FeederConfig feederConfig)
        {
            _feedJarView = feedJarView;
            _feedManager = feedManager;

            _feedManager._normalizedTime += ChangeFeedJarPercentage;

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
            }, feederConfig);
        }

        private void ChangeFeedJarPercentage(float percentage)
        {
            _feedJarView.SetPercentOfReadiness(percentage);
        }
    }
}
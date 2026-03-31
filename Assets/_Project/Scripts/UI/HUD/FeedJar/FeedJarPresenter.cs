using System;
using _Project.Core.Interfaces;
using Core.Feed;

namespace UI.FeedJar
{
    public class FeedJarPresenter : IUIInit, IDisposable
    {
        private readonly FeedJarView _feedJarView;
        private readonly FeedManager _feedManager;

        public FeedJarPresenter(FeedJarView feedJarView, FeedManager feedManager)
        {
            _feedJarView = feedJarView;
            _feedManager = feedManager;
        }

        public void Init()
        {
            _feedManager.OnNormalizedTime += ChangeFeedJarPercentage;

            _feedJarView.Init(() =>
            {
                if (_feedManager.TryFeed()) _feedJarView.OnFeedUsed();
                else _feedJarView.PlayShakeAnimation();
            });
        }

        public void Dispose()
        {
            _feedManager.OnNormalizedTime -= ChangeFeedJarPercentage;
        }

        private void ChangeFeedJarPercentage(float percentage)
        {
            _feedJarView.SetPercentOfReadiness(percentage, _feedManager.ActiveCooldown);
        }
    }
}
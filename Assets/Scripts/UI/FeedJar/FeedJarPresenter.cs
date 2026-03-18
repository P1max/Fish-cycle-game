using Core.Feed;
using UnityEngine;

namespace UI.FeedJar
{
    public class FeedJarPresenter
    {
        private readonly FeedJarView _feedJarView;
        private readonly FeedManager _feedManager;

        public float CurrentPercentOfReadiness { get; private set; }

        public FeedJarPresenter(FeedJarView feedJarView, FeedManager feedManager)
        {
            _feedJarView = feedJarView;
            _feedManager = feedManager;

            _feedManager._normalizedTime += ChangeFeedJarPercentage;

            _feedJarView.Init(() =>
            {
                if (_feedManager.IsReady)
                {
                    _feedManager.TryFeed();
                    
                    Debug.Log("Кормим! Ням ням!");
                }
                else
                {
                    Debug.Log("Ещё не готова кормить!");
                    
                    _feedJarView.PlayShakeAnimation();
                }
            });
        }

        private void ChangeFeedJarPercentage(float percentage)
        {
            CurrentPercentOfReadiness = percentage;

            _feedJarView.SetPercentOfReadiness(percentage);
        }
    }
}
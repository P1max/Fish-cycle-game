using System;
using _Project.Core.Interfaces;
using Core.Game;

namespace UI
{
    public class FishesCounterPresenter : IUIInit, IDisposable
    {
        private readonly FishesCounterView _view;
        private readonly FishesManager _fishesManager;
        private readonly AquariumConfig _aquariumConfig;

        public FishesCounterPresenter(FishesCounterView view, FishesManager fishesManager, AquariumConfig aquariumConfig)
        {
            _view = view;
            _fishesManager = fishesManager;
            _aquariumConfig = aquariumConfig;
        }

        public void PlayLimitReachedAnimation() => _view.PlayLimitReachedAnimation();

        public void Init()
        {
            _fishesManager.OnFishCountChanged += _view.SetCurrentFishesCount;

            _view.Init();
            _view.SetCurrentFishesCount(0, _aquariumConfig.MaxFishCount);
        }

        public void Dispose()
        {
            _fishesManager.OnFishCountChanged -= _view.SetCurrentFishesCount;
        }
    }
}
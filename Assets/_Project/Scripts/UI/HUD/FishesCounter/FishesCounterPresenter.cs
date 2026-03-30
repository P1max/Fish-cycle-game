using Core.Game;
using Zenject;

namespace UI
{
    public class FishesCounterPresenter : IInitializable
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

        public void Initialize()
        {
            _fishesManager.OnFishCountChanged += _view.SetCurrentFishesCount;
            
            _view.SetCurrentFishesCount(0, _aquariumConfig.MaxFishCount);
        }

        public void PlayLimitReachedAnimation() => _view.PlayLimitReachedAnimation();
    }
}
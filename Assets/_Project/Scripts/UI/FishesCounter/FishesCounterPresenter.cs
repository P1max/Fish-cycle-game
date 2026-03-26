using Core.Game;

namespace UI
{
    public class FishesCounterPresenter
    {
        private readonly FishesCounterView _view;
        private readonly FishesManager _fishesManager;
        
        public FishesCounterPresenter(FishesCounterView view, FishesManager fishesManager, AquariumConfig aquariumConfig)
        {
            _view = view;
            _fishesManager = fishesManager;
            
            _view.Init(aquariumConfig.MaxFishCount);
            
            fishesManager.OnFishCountChanged += _view.SetCurrentFishesCount;
        }

        public void PlayLimitReachedAnimation() => _view.PlayLimitReachedAnimation();
    }
}
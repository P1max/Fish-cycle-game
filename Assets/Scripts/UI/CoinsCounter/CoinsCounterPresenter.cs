using Core.Game;

namespace UI.MoneyCounter
{
    public class CoinsCounterPresenter
    {
        private readonly CoinsCounterView _view;
        private readonly BalanceManager _balanceManager;

        public CoinsCounterPresenter(CoinsCounterView view, BalanceManager balanceManager)
        {
            _view = view;
            _balanceManager = balanceManager;

            _balanceManager.OnCoinsCountChanged += _view.SetCurrentCoinsCount;
            
            _view.SetCurrentCoinsCount(_balanceManager.CurrentCoinsCount);
        }
    }
}
using Core.Game;
using Zenject;

namespace UI.MoneyCounter
{
    public class CoinsCounterPresenter : IInitializable
    {
        private readonly CoinsCounterView _view;
        private readonly BalanceManager _balanceManager;

        public CoinsCounterPresenter(CoinsCounterView view, BalanceManager balanceManager)
        {
            _view = view;
            _balanceManager = balanceManager;
        }

        public void Initialize()
        {
            _balanceManager.OnCoinsCountChanged += UpdateView;

            _view.SetCurrentCoinsCount(_balanceManager.CurrentCoinsCount, true);
        }

        private void UpdateView(int count)
        {
            _view.SetCurrentCoinsCount(count);
        }

        public void PlayNotEnoughMoneyAnimation() => _view.PlayNotEnoughMoneyAnimation();
    }
}
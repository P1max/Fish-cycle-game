using System;
using _Project.Core.Interfaces;
using Core.Game;

namespace UI.MoneyCounter
{
    public class CoinsCounterPresenter : IUIInit, IDisposable
    {
        private readonly CoinsCounterView _view;
        private readonly BalanceManager _balanceManager;

        public CoinsCounterPresenter(CoinsCounterView view, BalanceManager balanceManager)
        {
            _view = view;
            _balanceManager = balanceManager;
        }

        private void UpdateView(int count) => _view.SetCurrentCoinsCount(count);

        public void PlayNotEnoughMoneyAnimation() => _view.PlayNotEnoughMoneyAnimation();

        public void Init()
        {
            _balanceManager.OnCoinsCountChanged += UpdateView;

            _view.Init();
            _view.SetCurrentCoinsCount(_balanceManager.CurrentCoinsCount, true);
        }

        public void Dispose()
        {
            _balanceManager.OnCoinsCountChanged -= UpdateView;
        }
    }
}
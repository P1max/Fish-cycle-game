using System;
using System.Collections.Generic;
using _Project.Core.Interfaces;
using Core.Configs;
using Core.Game;
using UI.MoneyCounter;
using UnityEngine;
using Zenject;

namespace UI.Conveyor
{
    public class ConveyorPresenter : IUIInit, IDisposable, ITickable
    {
        private readonly ConveyorView _view;
        private readonly ConveyorConfig _config;
        private readonly StoreManager _storeManager;
        private readonly BalanceManager _balanceManager;
        private readonly FishesCounterPresenter _fishesCounterPresenter;
        private readonly CoinsCounterPresenter _coinsCounterPresenter;
        private readonly Dictionary<FishItemView, SlotData> _slots;

        private float _lastKnownSpacing;

        public ConveyorPresenter(
            ConveyorView view,
            ConveyorConfig config,
            StoreManager storeManager,
            BalanceManager balanceManager,
            FishesCounterPresenter fishesCounterPresenter,
            CoinsCounterPresenter coinsCounterPresenter)
        {
            _view = view;
            _config = config;
            _storeManager = storeManager;
            _balanceManager = balanceManager;
            _fishesCounterPresenter = fishesCounterPresenter;
            _coinsCounterPresenter = coinsCounterPresenter;

            _slots = new Dictionary<FishItemView, SlotData>();
        }

        private void HandleBalanceChanged(int newBalance) => _view.RefreshOffscreenItems();

        private void HandleRerollRequested(FishItemView view)
        {
            if (_slots.TryGetValue(view, out var data) && data.Lot != null)
            {
                _storeManager.DismissLot(data.Lot); // Списываем старую рыбу
            }
            else
            {
                data = new SlotData();
                _slots[view] = data;
            }

            data.Lot = _storeManager.RequestNewLotAndRegister();
            data.IsVisuallyPurchased = false;

            var fishConfig = _storeManager.GetFishConfig(data.Lot.FishId);
            var lifeTime = Mathf.RoundToInt(fishConfig.LifetimeSeconds * data.Lot.Quality);
            var income = Mathf.RoundToInt(fishConfig.IncomeCoins * data.Lot.Quality);

            view.SetData(fishConfig.Sprite, lifeTime, income, data.Lot.Price);
        }

        private void HandleItemClicked(FishItemView view)
        {
            if (!_slots.TryGetValue(view, out var data) || data.Lot == null) return;

            var success = _storeManager.TryBuyFish(data.Lot);

            if (success)
            {
                view.SetPurchasedState();
                data.IsVisuallyPurchased = true;
                _view.RefreshOffscreenItems();
            }
            else
            {
                view.PlayShakeAnimation();
            }
        }

        public void Tick()
        {
            foreach (var (view, data) in _slots)
            {
                if (data.Lot == null) continue;

                if (data.Lot.IsPurchased && !data.IsVisuallyPurchased)
                {
                    view.SetPurchasedState();
                    data.IsVisuallyPurchased = true;
                }

                if (!data.Lot.IsVisible && _view.IsViewVisible(view))
                    data.Lot.IsVisible = true;
            }

            if (!Mathf.Approximately(_config.ItemSpacing, _lastKnownSpacing))
            {
                _lastKnownSpacing = _config.ItemSpacing;
                _view.UpdateSpacing(_lastKnownSpacing);
            }
        }

        public void Init()
        {
            _view.OnItemRerollRequested += HandleRerollRequested;
            _view.OnItemClicked += HandleItemClicked;

            _balanceManager.OnCoinsCountChanged += HandleBalanceChanged;
            _storeManager.OnPurchaseFailedNotEnoughMoney += _coinsCounterPresenter.PlayNotEnoughMoneyAnimation;
            _storeManager.OnPurchaseFailedAquariumFull += _fishesCounterPresenter.PlayLimitReachedAnimation;

            _lastKnownSpacing = _config.ItemSpacing;

            _view.Init(_config.ConveyorSpeed, _config.ItemSpacing);
        }

        public void Dispose()
        {
            _view.OnItemRerollRequested -= HandleRerollRequested;
            _view.OnItemClicked -= HandleItemClicked;

            if (_balanceManager != null)
                _balanceManager.OnCoinsCountChanged -= HandleBalanceChanged;

            if (_storeManager != null)
            {
                _storeManager.OnPurchaseFailedNotEnoughMoney -= _coinsCounterPresenter.PlayNotEnoughMoneyAnimation;
                _storeManager.OnPurchaseFailedAquariumFull -= _fishesCounterPresenter.PlayLimitReachedAnimation;
            }
        }

        #region class

        private class SlotData
        {
            public StoreLot Lot;
            public bool IsVisuallyPurchased;
        }

        #endregion
    }
}
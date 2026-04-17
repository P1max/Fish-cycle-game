using System;
using System.Collections.Generic;
using System.Linq;
using _Project.Core.Interfaces;
using Core.Configs;
using Core.Loaders;
using Features.BotBalancer;
using UnityEngine;
using Zenject;
using Random = UnityEngine.Random;

namespace Core.Game
{
    public class StoreManager : IGameplayInit, ITickable
    {
        private readonly ConveyorConfig _config;
        private readonly BalanceManager _balanceManager;
        private readonly FishesManager _aquarium;
        private readonly FishesConfigsLoader _fishesConfigsLoader;
        private readonly BotSetupService _botSetup;
        private readonly IConveyorMetricsProvider _conveyorMetricsProvider;

        private List<string> _allFishIds;
        private List<string> _freeFishIds;
        private List<StoreLot> _availableLots;
        private float _timeSinceLastRoll;
        private float _rollInterval;

        public IReadOnlyList<StoreLot> AvailableLots => _availableLots;

        public event Action OnPurchaseFailedNotEnoughMoney;
        public event Action OnPurchaseFailedAquariumFull;
        public event Action OnFishBought;

        public StoreManager(
            ConveyorConfig config,
            BalanceManager balanceManager,
            FishesManager aquarium,
            FishesConfigsLoader fishesConfigsLoader,
            BotSetupService botSetup,
            IConveyorMetricsProvider conveyorMetricsProvider)
        {
            _config = config;
            _balanceManager = balanceManager;
            _aquarium = aquarium;
            _fishesConfigsLoader = fishesConfigsLoader;
            _botSetup = botSetup;
            _conveyorMetricsProvider = conveyorMetricsProvider;
        }

        private void ShiftLogicalConveyor()
        {
            if (_availableLots.Count > 0) _availableLots.RemoveAt(0);

            RequestNewLotAndRegister();
        }

        public FishConfig GetFishConfig(string fishId) => _fishesConfigsLoader.LoadedFishesDict[fishId];

        public StoreLot RequestNewLotAndRegister()
        {
            var isFree = _balanceManager.CurrentCoinsCount <= _config.CoinsForFreeFish &&
                         _aquarium.CurrentFishCount < _config.FishesCountForFreeFish;

            var targetPool = isFree ? _freeFishIds : _allFishIds;
            var randomId = targetPool[Random.Range(0, targetPool.Count)];
            var baseConfig = _fishesConfigsLoader.LoadedFishesDict[randomId];

            var quality = isFree
                ? Random.Range(_config.FreeQualityRange.x, _config.FreeQualityRange.y)
                : _config.QualityCurve.Evaluate(_balanceManager.CurrentCoinsCount);

            var price = isFree ? 0 : Mathf.RoundToInt(baseConfig.Price * quality);
            var isVisible = _botSetup.IsUIDisabled;
            var newLot = new StoreLot(randomId, quality, price, isVisible);

            _availableLots.Add(newLot);

            return newLot;
        }

        public void DismissLot(StoreLot lot)
        {
            if (lot != null && _availableLots.Contains(lot))
                _availableLots.Remove(lot);
        }

        public bool TryBuyFish(StoreLot lot)
        {
            if (lot.IsPurchased || !_availableLots.Contains(lot)) return false;

            if (!_aquarium.CanAddFish)
            {
                OnPurchaseFailedAquariumFull?.Invoke();

                return false;
            }

            if (!_balanceManager.TrySpendCoins(lot.Price))
            {
                OnPurchaseFailedNotEnoughMoney?.Invoke();

                return false;
            }

            if (_aquarium.TryAddFish(lot.FishId, lot.Quality))
            {
                lot.MarkAsPurchased();

                OnFishBought?.Invoke();
            }

            return true;
        }

        public void Tick()
        {
            if (!_botSetup.IsUIDisabled) return;

            _timeSinceLastRoll += Time.deltaTime;

            if (_timeSinceLastRoll >= _rollInterval)
            {
                _timeSinceLastRoll = 0f;

                ShiftLogicalConveyor();
            }
        }

        public void Init()
        {
            _availableLots = new List<StoreLot>();

            _allFishIds = _fishesConfigsLoader.LoadedFishesDict.Keys.ToList();

            if (_allFishIds.Count == 0) return;

            _freeFishIds = _fishesConfigsLoader.LoadedFishesDict.Where(k => k.Value.CanBeFree)
                .Select(k => k.Key).ToList();

            if (_freeFishIds.Count == 0)
            {
                Debug.LogWarning("Отсутствуют рыбы для бесплатного приобретения");

                _freeFishIds = _allFishIds;
            }

            var exactStepDistance = _conveyorMetricsProvider.GetExactStepDistance(_config.ItemSpacing);

            _rollInterval = exactStepDistance / _config.ConveyorSpeed;

            if (_botSetup.IsUIDisabled)
            {
                var startingSlots = _conveyorMetricsProvider.GetMaxItemsNeeded();

                for (var i = 0; i < startingSlots; i++)
                    RequestNewLotAndRegister();
            }
        }
    }
}
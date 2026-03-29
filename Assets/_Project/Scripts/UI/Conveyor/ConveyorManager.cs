using System.Collections.Generic;
using System.Linq;
using Core.Configs;
using Core.Game;
using Core.Loaders;
using UI.MoneyCounter;
using UnityEngine;
using Zenject;
using Random = UnityEngine.Random;

namespace UI.Conveyor
{
    public class ConveyorManager : MonoBehaviour
    {
        [SerializeField] private RectTransform _container;

        [Inject] private ConveyorConfig _config;
        [Inject] private BalanceManager _balanceManager;
        [Inject] private FishesManager _aquarium;
        [Inject] private FishesLoader _fishesLoader;
        [Inject] private FishesCounterPresenter _fishesCounterPresenter;
        [Inject] private CoinsCounterPresenter _coinsCounterPresenter;

        private List<FishItemView> _items;
        private List<string> _allFishIds;
        private FishItemView _prefab;
        private RectTransform _viewport;
        private float _stepDistance;
        private float _itemWidth;
        private float _targetHeight;
        private float _lastKnownSpacing;
        private float _despawnThresholdX;

        private void Awake() => _prefab = Resources.Load<FishItemView>("Prefabs/FishItem");

        private void OnBalanceManagerChanged(int newBalance) => RefreshOffscreenItems();

        private void Start()
        {
            _items = new List<FishItemView>();
            _allFishIds = _fishesLoader.LoadedFishesDict.Keys.ToList();

            if (_allFishIds.Count == 0)
            {
                Debug.LogError("[ConveyorManager] Нет доступных рыб в словаре лоадера!");

                return;
            }

            _viewport = _container.parent.GetComponent<RectTransform>();

            Canvas.ForceUpdateCanvases();

            var prefabRect = _prefab.Rect;
            var originalWidth = prefabRect.rect.width;
            var originalHeight = prefabRect.rect.height;
            var aspectRatio = originalWidth / originalHeight;

            _targetHeight = _viewport.rect.height;
            _itemWidth = _targetHeight * aspectRatio;
            _despawnThresholdX = _viewport.rect.xMax + (_itemWidth / 2f);

            InitializeConveyor();

            _balanceManager.OnCoinsCountChanged += OnBalanceManagerChanged;
        }

        private void Update()
        {
            if (_items == null || _items.Count == 0) return;

            if (!Mathf.Approximately(_config.ItemSpacing, _lastKnownSpacing))
            {
                _lastKnownSpacing = _config.ItemSpacing;
                _stepDistance = _itemWidth + _config.ItemSpacing;

                UpdateItemsPositions();
            }

            _container.anchoredPosition += Vector2.right * (_config.ConveyorSpeed * Time.deltaTime);

            CheckCarousel();
        }

        private void UpdateItemsPositions()
        {
            var basePosX = _items[0].Rect.anchoredPosition.x;

            for (var i = 1; i < _items.Count; i++)
            {
                var rect = _items[i].Rect;

                rect.anchoredPosition = new Vector2(basePosX - i * _stepDistance, rect.anchoredPosition.y);
            }
        }

        private void InitializeConveyor()
        {
            _lastKnownSpacing = _config.ItemSpacing;
            _stepDistance = _itemWidth + _config.ItemSpacing;

            var maxItemsNeeded = Mathf.CeilToInt(_viewport.rect.width / _itemWidth) + 1;
            var startX = _viewport.rect.xMin - (_itemWidth / 2f);

            _container.anchoredPosition = new Vector2(0, _container.anchoredPosition.y);

            for (var i = 0; i < maxItemsNeeded; i++)
            {
                var view = Instantiate(_prefab, _container, false);
                var rect = view.GetComponent<RectTransform>();

                rect.sizeDelta = new Vector2(_itemWidth, _targetHeight);
                rect.anchoredPosition = new Vector2(startX - (i * _stepDistance), 0);

                ReRollItem(view);

                _items.Add(view);
            }
        }

        private void CheckCarousel()
        {
            var rightmostItem = _items[0];
            var localPosInViewport = _viewport.InverseTransformPoint(rightmostItem.transform.position);

            if (localPosInViewport.x > _despawnThresholdX)
            {
                var leftmostItem = _items[^1];
                var rect = rightmostItem.Rect;
                var leftmostPos = leftmostItem.Rect.anchoredPosition.x;

                rect.anchoredPosition = new Vector2(leftmostPos - _stepDistance, rect.anchoredPosition.y);

                ReRollItem(rightmostItem);

                _items.RemoveAt(0);
                _items.Add(rightmostItem);
            }
        }

        private void RefreshOffscreenItems()
        {
            if (_items == null || _items.Count == 0) return;

            foreach (var item in _items)
            {
                var localPosInViewport = _viewport.InverseTransformPoint(item.transform.position);
                var rightEdge = localPosInViewport.x + (_itemWidth / 2f);

                if (rightEdge <= _viewport.rect.xMin - 5f)
                    ReRollItem(item);
            }
        }

        private float CalculateQuality(int coins)
        {
            if (coins <= _config.CoinsForFreeFish && _aquarium.CurrentFishCount < _config.FishesCountForFreeFish)
                return Random.Range(_config.FreeQualityRange.x, _config.FreeQualityRange.y);
            
            return _config.QualityCurve.Evaluate(coins);
        }

        private void ReRollItem(FishItemView view)
        {
            var randomId = _allFishIds[Random.Range(0, _allFishIds.Count)];
            var baseConfig = _fishesLoader.LoadedFishesDict[randomId];

            var quality = CalculateQuality(_balanceManager.CurrentCoinsCount);
            var lifeTime = baseConfig.LifetimeSeconds * quality;
            var income = Mathf.RoundToInt(baseConfig.IncomeCoins * quality);
            var price = quality <= _config.FreeQualityRange.y ? 0 : Mathf.RoundToInt(baseConfig.Price * quality);

            view.SetData(baseConfig.Sprite, lifeTime, income, price);

            view.SetButtonAction(isPurchased =>
            {
                if (!isPurchased) TryBuyFish(view, randomId, quality, price);
            });
        }

        private void TryBuyFish(FishItemView view, string fishId, float quality, int price)
        {
            if (!_aquarium.CanAddFish)
            {
                _fishesCounterPresenter.PlayLimitReachedAnimation();
                view.PlayShakeAnimation();

                return;
            }

            if (!_balanceManager.TrySpendCoins(price))
            {
                _coinsCounterPresenter.PlayNotEnoughMoneyAnimation();
                view.PlayShakeAnimation();

                return;
            }

            _aquarium.TryAddFish(fishId, quality);
            view.SetPurchasedState();

            RefreshOffscreenItems();
        }
    }
}
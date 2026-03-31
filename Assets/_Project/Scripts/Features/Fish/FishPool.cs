using System;
using System.Collections.Generic;
using Core.Game;
using Core.Game.Upgrade;
using Core.Loaders;
using DG.Tweening;
using UnityEngine;
using Zenject;

namespace Spawners
{
    public class FishPool : BaseEntityPool<FishEntity>, IDisposable
    {
        [Inject] private AquariumBoundsManager _aquariumBoundsManager;
        [Inject] private UpgradeManager _upgradeManager;
        [Inject] private CommonFishConfig _commonFishConfig;
        [Inject] private AquariumConfig _aquariumConfig;
        [Inject] private FishesConfigsLoader _fishesConfigsLoader;
        [Inject] private BreedManager _breedManager;
        [Inject] private CoinsPool _coinsPool;
        [Inject] private FoodPool _foodPool;

        private Dictionary<Collider2D, FishEntity> _fishesCache;
        private float _defaultScale;
        private Sequence _scaleAnimation;

        public List<FishEntity> ActiveFishes => ActiveItems;

        private void HandleUpgrade(UpgradesConfig.LevelData newData)
        {
            transform.DOKill(true);
            transform.DOScale(Vector3.one * (_defaultScale * newData.NewAquariumScale), 1.5f).SetEase(Ease.InOutSine);
        }

        protected override void LoadPrefab()
        {
            Prefab = Resources.Load<FishEntity>("Prefabs/Entities/Fish");
        }

        protected override void OnItemCreated(FishEntity fish)
        {
            var col = fish.GetComponent<Collider2D>();

            _fishesCache.Add(col, fish);

            fish.Init(_fishesCache, _foodPool, col, _commonFishConfig, _coinsPool, _aquariumBoundsManager, _breedManager,
                _fishesConfigsLoader);

            fish.OnReturnToPool += ReturnToPool;
        }

        public override void Init()
        {
            base.Init();

            _defaultScale = _aquariumConfig.DefaultEntitiesScale * _aquariumConfig.FishesDefaultScale;
            transform.localScale = Vector3.one * _defaultScale;

            _fishesCache = new Dictionary<Collider2D, FishEntity>();

            _upgradeManager.OnAquariumUpgrade += HandleUpgrade;
        }

        public void Dispose()
        {
            if (_upgradeManager != null)
                _upgradeManager.OnAquariumUpgrade -= HandleUpgrade;
        }
    }
}
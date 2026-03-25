using System.Collections.Generic;
using Core.Game;
using Core.Loaders;
using UnityEngine;
using Zenject;

namespace Spawners
{
    public class FishPool : BaseEntityPool<FishEntity>
    {
        [Inject] private FoodPool _foodPool;
        [Inject] private CoinsPool _coinsPool;
        [Inject] private CommonFishConfig _commonFishConfig;
        [Inject] private FishesLoader _fishesLoader;
        [Inject] private AquariumBoundsManager _aquariumBoundsManager;
        [Inject] private BreedManager _breedManager;

        private Dictionary<Collider2D, FishEntity> _fishesCache;

        protected override void Awake()
        {
            base.Awake();

            _fishesCache = new Dictionary<Collider2D, FishEntity>();
        }

        protected override void LoadPrefab()
        {
            Prefab = Resources.Load<FishEntity>("Prefabs/Fish");
        }

        protected override void OnItemCreated(FishEntity fish)
        {
            var col = fish.GetComponent<Collider2D>();

            _fishesCache.Add(col, fish);

            fish.Init(_fishesCache, _foodPool, col, _commonFishConfig, _coinsPool, _aquariumBoundsManager, _breedManager, _fishesLoader);
            fish.OnReturnToPool += ReturnToPool;
        }
    }
}
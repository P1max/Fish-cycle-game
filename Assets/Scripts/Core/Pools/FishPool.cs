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

        public FishEntity GetFish(string fishId)
        {
            var targetConfig = _fishesLoader.LoadedFishesDict[fishId];

            if (targetConfig == null)
            {
                Debug.LogError($"Пул не нашел конфиг рыбы с ID: {fishId}");

                return null;
            }

            var fish = Get();

            fish.SetConfig(targetConfig);

            return fish;
        }

        protected override void OnItemCreated(FishEntity fish)
        {
            var col = fish.GetComponent<Collider2D>();

            _fishesCache.Add(col, fish);

            fish.Init(_fishesCache, _foodPool, col, _commonFishConfig, _coinsPool, _aquariumBoundsManager, _breedManager);

            fish.OnReturnedToPool += ReturnToPool;
        }
    }
}
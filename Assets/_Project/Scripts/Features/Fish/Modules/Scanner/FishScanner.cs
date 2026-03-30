using System.Collections.Generic;
using Core.Feed;
using Spawners;
using UnityEngine;

namespace Core.Fish.Modules
{
    public class FishScanner
    {
        private static readonly Collider2D[] _buffer = new Collider2D[100];

        private readonly FishEntity _fish;
        private readonly IReadOnlyDictionary<Collider2D, FishEntity> _fishesCache;
        private readonly FoodPool _foodPool;
        private readonly CoinsPool _coinsPool;
        private readonly ContactFilter2D _filter;

        public readonly List<FishEntity> NearbyAliveFishes;
        public readonly List<FishEntity> NearbyDeadFishes;
        public readonly List<FoodPiece> NearbyFood;
        public readonly List<Coin.Coin> NearbyCoins;

        public FishScanner(FishEntity fish, IReadOnlyDictionary<Collider2D, FishEntity> fishesCache, FoodPool foodPool,
            CoinsPool coinsPool)
        {
            _fish = fish;
            _fishesCache = fishesCache;
            _foodPool = foodPool;
            _coinsPool = coinsPool;
            _filter = new ContactFilter2D().NoFilter();

            NearbyAliveFishes = new List<FishEntity>();
            NearbyDeadFishes = new List<FishEntity>();
            NearbyFood = new List<FoodPiece>();
            NearbyCoins = new List<Coin.Coin>();
        }

        public void Tick()
        {
            NearbyAliveFishes.Clear();
            NearbyDeadFishes.Clear();
            NearbyFood.Clear();
            NearbyCoins.Clear();

            var maxRadius = Mathf.Max(
                _fish.CommonFishConfig.NeighborRadius,
                _fish.CommonFishConfig.FoodSearchRadius
            );

            var count = Physics2D.OverlapCircle(_fish.transform.position, maxRadius, _filter, _buffer);

            for (var i = 0; i < count; i++)
            {
                var col = _buffer[i];

                if (col.gameObject == _fish.gameObject) continue;

                if (_fishesCache.TryGetValue(col, out var otherFish))
                {
                    if (otherFish.IsAlive) NearbyAliveFishes.Add(otherFish);
                    else NearbyDeadFishes.Add(otherFish);
                }
                else if (_foodPool.FoodCache.TryGetValue(col, out var food) && !food.IsConsumed)
                {
                    NearbyFood.Add(food);
                }
                else if (_coinsPool.CoinsCache.TryGetValue(col, out var coin))
                {
                    NearbyCoins.Add(coin);
                }
            }
        }
    }
}
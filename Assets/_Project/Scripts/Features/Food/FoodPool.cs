using System.Collections.Generic;
using Core.Feed;
using UnityEngine;
using Zenject;

namespace Spawners
{
    public class FoodPool : BaseEntityPool<FoodPiece>
    {
        [Inject] private AquariumConfig _aquariumConfig;

        private Dictionary<Collider2D, FoodPiece> _foodCache;

        public IReadOnlyDictionary<Collider2D, FoodPiece> FoodCache => _foodCache;

        protected override void LoadPrefab()
        {
            Prefab = Resources.Load<FoodPiece>("Prefabs/Entities/Food");
        }

        protected override void OnItemCreated(FoodPiece food)
        {
            food.Init(this);

            _foodCache.Add(food.GetComponent<Collider2D>(), food);
        }

        public override void Init()
        {
            base.Init();

            transform.localScale = Vector3.one * (_aquariumConfig.DefaultEntitiesScale * _aquariumConfig.FoodDefaultScale);
            _foodCache = new Dictionary<Collider2D, FoodPiece>();
        }
    }
}
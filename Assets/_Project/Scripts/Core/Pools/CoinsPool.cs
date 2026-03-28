using System.Collections.Generic;
using Core.Coin;
using Core.Game;
using UnityEngine;
using Zenject;

namespace Spawners
{
    public class CoinsPool : BaseEntityPool<Coin>
    {
        [Inject] private BalanceManager _balanceManager;
        [Inject] private AquariumConfig _aquariumConfig;
        
        private Dictionary<Collider2D, Coin> _coinsCache;
        
        public IReadOnlyDictionary<Collider2D, Coin> CoinsCache => _coinsCache;

        protected override void Awake()
        {
            base.Awake();

            transform.localScale = Vector3.one * _aquariumConfig.DefaultEntitiesScale * _aquariumConfig.CoinsDefaultScale;
            _coinsCache = new Dictionary<Collider2D, Coin>();
        }

        protected override void LoadPrefab()
        {
            Prefab = Resources.Load<Coin>("Prefabs/Coin");
        }
        
        protected override void OnItemCreated(Coin item)
        {
            item.Init(_balanceManager, this);
            
            _coinsCache.Add(item.GetComponent<Collider2D>(), item);
        }
        
        public void SpawnCoin(Vector2 position, int value)
        {
            var coin = Get(); 

            coin.Spawn(position, value);
        }
    }
}
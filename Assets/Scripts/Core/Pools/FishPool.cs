using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

namespace Spawners
{
    public class FishPool : MonoBehaviour
    {
        [Inject] private FoodPool _foodPool;
        [Inject] private CommonFishConfig _commonFishConfig;
        [Inject] private Dictionary<string, FishConfig> _fishConfigs;

        private Dictionary<Collider2D, FishEntity> _fishesCache;
        private LinkedList<FishEntity> _activeFishes;
        private LinkedList<FishEntity> _freeFishes;
        private FishEntity _fishPrefab;

        public FishEntity[] GetActiveFishes() => _activeFishes.ToArray();

        private void Awake()
        {
            _fishPrefab = Resources.Load<FishEntity>("Prefabs/Fish");

            _activeFishes = new LinkedList<FishEntity>();
            _freeFishes = new LinkedList<FishEntity>();
            _fishesCache = new Dictionary<Collider2D, FishEntity>();
        }

        public FishEntity GetFish(string fishId)
        {
            var targetConfig = _fishConfigs[fishId];

            if (targetConfig == null)
            {
                Debug.LogError($"Пул не нашел конфиг рыбы с ID: {fishId}");

                return null;
            }

            FishEntity fish;

            if (_freeFishes.Count > 0)
            {
                fish = _freeFishes.First.Value;
                _freeFishes.RemoveFirst();
            }
            else
            {
                fish = Instantiate(_fishPrefab, transform, true);

                var col = fish.GetComponent<Collider2D>();

                _fishesCache.Add(col, fish);

                fish.Init(_fishesCache, _foodPool, col, _commonFishConfig);

                fish.OnReadyToPool += ReturnFish;
            }

            fish.SetConfig(targetConfig);

            fish.gameObject.SetActive(true);
            _activeFishes.AddLast(fish);

            return fish;
        }

        public void ReturnFish(FishEntity fish)
        {
            fish.gameObject.SetActive(false);

            _activeFishes.Remove(fish);
            _freeFishes.AddLast(fish);
        }
    }
}
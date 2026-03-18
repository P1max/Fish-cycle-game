using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

namespace Spawners
{
    public class FishPool : MonoBehaviour
    {
        [SerializeField] private FishConfig _defaultConfig;
        
        [Inject] private FoodPool _foodPool;
        
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

        public FishEntity GetFish()
        {
            FishEntity fish;

            if (_freeFishes.Count > 0)
            {
                fish = _freeFishes.First.Value;
                _freeFishes.RemoveFirst();
            }
            else
            {
                fish = Instantiate(_fishPrefab, transform, true);

                fish.Init(_defaultConfig, _fishesCache, _foodPool);

                var col = fish.GetComponent<Collider2D>();
                
                _fishesCache.Add(col, fish);
            }

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
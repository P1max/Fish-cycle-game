using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Spawners
{
    public class FishPool : MonoBehaviour
    {
        private Dictionary<Collider2D, FishBoid> _fishesCache;
        private LinkedList<FishBoid> _activeFishes;
        private LinkedList<FishBoid> _freeFishes;
        private FishBoid _fishPrefab;

        public FishBoid[] GetActiveFishes() => _activeFishes.ToArray();

        private void Awake()
        {
            _fishPrefab = Resources.Load<FishBoid>("Prefabs/Fish");

            _activeFishes = new LinkedList<FishBoid>();
            _freeFishes = new LinkedList<FishBoid>();
            _fishesCache = new Dictionary<Collider2D, FishBoid>();
        }

        public FishBoid GetFish()
        {
            FishBoid fish;

            if (_freeFishes.Count > 0)
            {
                fish = _freeFishes.First.Value;
                _freeFishes.RemoveFirst();
            }
            else
            {
                fish = Instantiate(_fishPrefab, transform, true);

                fish.Init(_fishesCache);

                var col = fish.GetComponent<Collider2D>();
                
                _fishesCache.Add(col, fish);
            }

            fish.gameObject.SetActive(true);
            _activeFishes.AddLast(fish);

            return fish;
        }

        public void ReturnFish(FishBoid fish)
        {
            fish.gameObject.SetActive(false);

            _activeFishes.Remove(fish);
            _freeFishes.AddLast(fish);
        }
    }
}
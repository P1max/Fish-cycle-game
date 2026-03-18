using System.Collections.Generic;
using System.Linq;
using Core.Feed;
using UnityEngine;

namespace Spawners
{
    public class FoodPool : MonoBehaviour
    {
        private Dictionary<Collider2D, FoodPiece> _foodCache;
        private LinkedList<FoodPiece> _activeFood;
        private LinkedList<FoodPiece> _freeFood;
        private FoodPiece _foodPrefab;

        public FoodPiece[] GetActiveFood() => _activeFood.ToArray();

        public IReadOnlyDictionary<Collider2D, FoodPiece> FoodCache => _foodCache;

        private void Awake()
        {
            _foodPrefab = Resources.Load<FoodPiece>("Prefabs/Food");

            _activeFood = new LinkedList<FoodPiece>();
            _freeFood = new LinkedList<FoodPiece>();
            _foodCache = new Dictionary<Collider2D, FoodPiece>();
        }

        public FoodPiece GetFood()
        {
            FoodPiece food;

            if (_freeFood.Count > 0)
            {
                food = _freeFood.First.Value;
                _freeFood.RemoveFirst();
            }
            else
            {
                food = Instantiate(_foodPrefab, transform, true);

                food.Init(this);

                var col = food.GetComponent<Collider2D>();

                _foodCache.Add(col, food);
            }

            food.gameObject.SetActive(true);
            _activeFood.AddLast(food);

            return food;
        }

        public void ReturnFood(FoodPiece food)
        {
            food.gameObject.SetActive(false);

            _activeFood.Remove(food);
            _freeFood.AddLast(food);
        }
    }
}
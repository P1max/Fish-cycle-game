using Spawners;
using UnityEngine;

namespace Core.Fish.BoidStrategies
{
    public class FoodSteering : ISteeringBehavior
    {
        private static readonly Collider2D[] _overlapResults = new Collider2D[16];
        private readonly ContactFilter2D _filter;
        private readonly FoodPool _foodPool;

        public FoodSteering(FoodPool foodPool)
        {
            _foodPool = foodPool;
            _filter = new ContactFilter2D().NoFilter();
        }

        public Vector2 CalculateSteering(FishEntity fish)
        {
            if (fish.Hunger.CurrentHungerPercent < 20f)
                return Vector2.zero;

            var currentPos = (Vector2)fish.transform.position;
            var closestFoodPos = Vector2.zero;
            var minDistance = float.MaxValue;
            var foundFood = false;

            var count = Physics2D.OverlapCircle(currentPos, fish.BoidsConfig.FoodSearchRadius, _filter, _overlapResults);

            for (var i = 0; i < count; i++)
            {
                var col = _overlapResults[i];

                if (_foodPool.FoodCache.TryGetValue(col, out var foodPiece))
                {
                    if (foodPiece.IsConsumed) continue;

                    var distance = Vector2.Distance(currentPos, foodPiece.transform.position);

                    if (distance < minDistance)
                    {
                        minDistance = distance;
                        closestFoodPos = foodPiece.transform.position;
                        foundFood = true;
                    }
                }
            }

            if (foundFood)
            {
                var directionToFood = (closestFoodPos - currentPos).normalized;

                var hungerMultiplier = fish.Hunger.CurrentHungerPercent / 100f;

                return directionToFood * (fish.BoidsConfig.FoodWeight * hungerMultiplier);
            }

            return Vector2.zero;
        }
    }
}
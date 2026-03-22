using Core.Feed;
using Spawners;
using UnityEngine;

namespace Core.Fish.BoidStrategies
{
    public class FoodSteering : ISteeringBehavior
    {
        private static readonly Collider2D[] _overlapResults = new Collider2D[16];
        
        private readonly ContactFilter2D _filter;
        private readonly FoodPool _foodPool;
        private readonly Collider2D _collider;
        
        private float _eatCooldownTime;

        public FoodSteering(FoodPool foodPool, Collider2D collider)
        {
            _foodPool = foodPool;
            _collider = collider;
            _filter = new ContactFilter2D().NoFilter();
        }

        public Vector2 CalculateSteering(FishEntity fish)
        {
            fish.Movement.IsChasingFood = false;
            
            if (Time.time < _eatCooldownTime) 
                return Vector2.zero;

            if (fish.Hunger.CurrentHungerPercent < 20f)
                return Vector2.zero;

            FoodPiece targetFood = null;
            var currentPos = (Vector2)fish.transform.position;
            var minDistance = float.MaxValue;
            var count = Physics2D.OverlapCircle(currentPos, fish.CommonFishConfig.FoodSearchRadius, _filter, _overlapResults);

            for (var i = 0; i < count; i++)
            {
                var col = _overlapResults[i];

                if (_foodPool.FoodCache.TryGetValue(col, out var foodPiece))
                {
                    if (foodPiece.IsConsumed) continue;

                    if (_collider.bounds.Intersects(col.bounds))
                    {
                        foodPiece.Consume(fish);
                        _eatCooldownTime = Time.time + Random.Range(fish.CommonFishConfig.EatTimer.x, fish.CommonFishConfig.EatTimer.y); 
                        
                        return Vector2.zero;
                    }
                    
                    var distance = Vector2.Distance(currentPos, foodPiece.transform.position);

                    if (distance < minDistance)
                    {
                        minDistance = distance;
                        targetFood = foodPiece;
                    }
                }
            }

            if (targetFood != null)
            {
                fish.Movement.IsChasingFood = true;
                
                var directionToFood = (targetFood.transform.position - (Vector3)currentPos).normalized;
                var hungerMultiplier = fish.Hunger.CurrentHungerPercent / 100f;

                return directionToFood * (fish.CommonFishConfig.FoodWeight * hungerMultiplier);
            }

            return Vector2.zero;
        }
    }
}
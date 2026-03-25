using Core.Feed;
using UnityEngine;

namespace Core.Fish.BoidStrategies
{
    public class FoodSteering : ISteeringBehavior
    {
        private readonly Collider2D _collider;

        private float _eatCooldownTime;

        public FoodSteering(Collider2D collider)
        {
            _collider = collider;
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

            foreach (var foodPiece in fish.Scanner.NearbyFood)
            {
                if (foodPiece.IsConsumed) continue;

                var distance = Vector2.Distance(currentPos, foodPiece.transform.position);

                if (distance > fish.CommonFishConfig.FoodSearchRadius) continue;

                if (_collider.bounds.Intersects(foodPiece.GetComponent<Collider2D>().bounds))
                {
                    foodPiece.Consume(fish);
                    _eatCooldownTime = Time.time + Random.Range(fish.CommonFishConfig.EatTimer.x, fish.CommonFishConfig.EatTimer.y);

                    return Vector2.zero;
                }

                if (distance < minDistance)
                {
                    minDistance = distance;
                    targetFood = foodPiece;
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
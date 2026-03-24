using Spawners;
using UnityEngine;

namespace Core.Fish.BoidStrategies
{
    public class ObstacleSteering : ISteeringBehavior
    {
        private static readonly Collider2D[] _overlapResults = new Collider2D[100];
        private readonly ContactFilter2D _filter;
        private readonly CoinsPool _coinsPool;

        public ObstacleSteering(CoinsPool coinsPool)
        {
            _filter = new ContactFilter2D().NoFilter();
            _coinsPool = coinsPool;
        }

        public Vector2 CalculateSteering(FishEntity fish)
        {
            var avoidance = Vector2.zero;
            var obstacleCount = 0;
            var count = Physics2D.OverlapCircle(fish.transform.position, fish.CommonFishConfig.SeparationRadius, _filter, _overlapResults);

            for (var i = 0; i < count; i++)
            {
                var col = _overlapResults[i];

                if (col.gameObject == fish.gameObject) continue;

                var isObstacle = false;

                if (fish.Movement.FishesCache.TryGetValue(col, out var otherFish))
                {
                    if (!otherFish.IsAlive)
                        isObstacle = true;
                }
                else if (_coinsPool.CoinsCache.TryGetValue(col, out var coin))
                {
                    isObstacle = true;
                }

                if (isObstacle)
                {
                    var diff = (Vector2)fish.transform.position - (Vector2)col.transform.position;
                    var distance = diff.magnitude;

                    if (distance < fish.CommonFishConfig.SeparationRadius && distance > 0)
                    {
                        var repulsionForce = 1f - (distance / fish.CommonFishConfig.SeparationRadius);

                        avoidance += diff.normalized * repulsionForce;
                        obstacleCount++;
                    }
                }
            }

            if (obstacleCount > 0)
            {
                avoidance = (avoidance / obstacleCount) * fish.CommonFishConfig.ObstacleAvoidanceWeight;
            }

            return avoidance;
        }
    }
}
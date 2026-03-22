using UnityEngine;

namespace Core.Fish.BoidStrategies
{
    public class ObstacleSteering : ISteeringBehavior
    {
        private static readonly Collider2D[] _overlapResults = new Collider2D[64];
        private readonly ContactFilter2D _filter;

        public ObstacleSteering()
        {
            _filter = new ContactFilter2D().NoFilter();
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
                    if (!otherFish.IsAlive)
                        isObstacle = true;

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
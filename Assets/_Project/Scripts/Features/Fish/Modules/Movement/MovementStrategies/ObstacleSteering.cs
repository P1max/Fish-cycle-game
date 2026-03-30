using UnityEngine;

namespace Core.Fish.BoidStrategies
{
    public class ObstacleSteering : ISteeringBehavior
    {
        public Vector2 CalculateSteering(FishEntity fish)
        {
            var avoidance = Vector2.zero;
            var obstacleCount = 0;

            foreach (var deadFish in fish.Scanner.NearbyDeadFishes)
            {
                avoidance += CalculateRepulsion(fish, deadFish.transform.position, ref obstacleCount);
            }

            foreach (var coin in fish.Scanner.NearbyCoins)
            {
                avoidance += CalculateRepulsion(fish, coin.transform.position, ref obstacleCount);
            }

            if (obstacleCount > 0)
            {
                avoidance = (avoidance / obstacleCount) * fish.CommonFishConfig.ObstacleAvoidanceWeight;
            }

            return avoidance;
        }

        private Vector2 CalculateRepulsion(FishEntity fish, Vector3 targetPos, ref int obstacleCount)
        {
            var diff = fish.transform.position - targetPos;
            var distance = diff.magnitude;

            if (distance < fish.CommonFishConfig.SeparationRadius && distance > 0)
            {
                obstacleCount++;

                var repulsionForce = 1f - (distance / fish.CommonFishConfig.SeparationRadius);

                return diff.normalized * repulsionForce;
            }

            return Vector2.zero;
        }
    }
}
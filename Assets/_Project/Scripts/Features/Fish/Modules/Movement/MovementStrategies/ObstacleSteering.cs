using UI.FeedJar;
using UnityEngine;

namespace Core.Fish.BoidStrategies
{
    public class ObstacleSteering : ISteeringBehavior
    {
        private readonly FeedJarView _feedJarView;
        private readonly Camera _camera;

        public ObstacleSteering(FeedJarView feedJarView)
        {
            _feedJarView = feedJarView;
            _camera = Camera.main;
        }

        public Vector2 CalculateSteering(FishEntity fish)
        {
            var avoidance = Vector2.zero;
            var obstacleCount = 0;

            foreach (var deadFish in fish.Scanner.NearbyDeadFishes)
            {
                avoidance += CalculateRepulsion(fish, deadFish.transform.position, fish.CommonFishConfig.SeparationRadius,
                    ref obstacleCount);
            }

            foreach (var coin in fish.Scanner.NearbyCoins)
            {
                avoidance += CalculateRepulsion(fish, coin.transform.position, fish.CommonFishConfig.SeparationRadius, ref obstacleCount);
            }

            if (_feedJarView != null && _feedJarView.gameObject.activeInHierarchy && _camera != null)
            {
                var feederPos = _feedJarView.GetWorldPosition(_camera);

                avoidance += CalculateRepulsion(fish, feederPos, _feedJarView.ObstacleRadius, ref obstacleCount);
            }

            if (obstacleCount > 0)
            {
                avoidance = (avoidance / obstacleCount) * fish.CommonFishConfig.ObstacleAvoidanceWeight;
            }

            return avoidance;
        }

        private Vector2 CalculateRepulsion(FishEntity fish, Vector3 targetPos, float radius, ref int obstacleCount)
        {
            var diff = fish.transform.position - targetPos;
            var distance = diff.magnitude;

            if (distance < radius && distance > 0)
            {
                obstacleCount++;
                var repulsionForce = 1f - (distance / radius);

                return diff.normalized * repulsionForce;
            }

            return Vector2.zero;
        }
    }
}
using UnityEngine;

namespace Core.Fish.BoidStrategies
{
    public class FleeSteering : ISteeringBehavior
    {
        public Vector2 CalculateSteering(FishEntity fish)
        {
            if (fish.Movement.ScareTimer <= 0) return Vector2.zero;

            var diff = (Vector2)fish.transform.position - fish.Movement.ScareSource;
            var distance = diff.magnitude;

            if (distance < fish.CommonFishConfig.ScareRadius && distance > 0)
            {
                var force = 1f - (distance / fish.CommonFishConfig.ScareRadius);

                return diff.normalized * (force * fish.CommonFishConfig.ScareWeight);
            }

            return Vector2.zero;
        }
    }
}
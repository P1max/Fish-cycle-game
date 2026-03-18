using UnityEngine;

namespace Core.Fish.BoidStrategies
{
    public interface ISteeringBehavior
    {
        Vector2 CalculateSteering(FishMovement fish);
    }
}
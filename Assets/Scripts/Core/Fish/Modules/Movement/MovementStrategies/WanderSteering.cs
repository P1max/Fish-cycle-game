using Core.Fish.BoidStrategies;
using UnityEngine;

public class WanderSteering : ISteeringBehavior
{
    private Vector2 _wanderTarget;
    private float _nextChangeTime;

    public Vector2 CalculateSteering(FishEntity fish)
    {
        // Меняем цель, если пришло время ИЛИ если рыбка у края (чтобы она хотела уплыть от края аквариума)
        if (Time.time >= _nextChangeTime || fish.Movement.IsNearEdge)
        {
            _wanderTarget = Random.insideUnitCircle.normalized;
            _nextChangeTime = Time.time + 2f + Random.Range(0f, 2f);
        }

        return _wanderTarget * fish.Config.WanderWeight;
    }
}
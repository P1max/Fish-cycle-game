using Core.Fish.BoidStrategies;
using UnityEngine;

public class WanderSteering : ISteeringBehavior
{
    private Vector2 _wanderTarget;
    private float _nextChangeTime;

    public Vector2 CalculateSteering(FishEntity fish)
    {
        if (Time.time >= _nextChangeTime)
        {
            _wanderTarget = Random.insideUnitCircle.normalized;
            _nextChangeTime = Time.time + Random.Range(1.5f, 3f);
        }

        if (fish.Movement.IsNearEdge)
        {
            var viewportPos = Camera.main.WorldToViewportPoint(fish.transform.position);
            var marginX = fish.CommonFishConfig.MarginXSides;
            var marginTop = fish.CommonFishConfig.MarginTop;
            var marginBottom = fish.CommonFishConfig.MarginBottom;

            bool desireChanged = false;

            if (viewportPos.x < marginX && _wanderTarget.x < 0)
            {
                _wanderTarget.x *= -1;
                _wanderTarget.y *= Random.Range(-1f, 1f);
                desireChanged = true;
            }
            else if (viewportPos.x > 1f - marginX && _wanderTarget.x > 0)
            {
                _wanderTarget.x *= -1;
                _wanderTarget.y *= Random.Range(-1f, 1f);
                desireChanged = true;
            }

            if (viewportPos.y < marginBottom && _wanderTarget.y < 0)
            {
                _wanderTarget.y *= -1;
                _wanderTarget.x *= Random.Range(-1f, 1f);
                desireChanged = true;
            }
            else if (viewportPos.y > 1f - marginTop && _wanderTarget.y > 0)
            {
                _wanderTarget.y *= -1;
                _wanderTarget.x *= Random.Range(-1f, 1f);
                desireChanged = true;
            }

            if (desireChanged)
            {
                _wanderTarget = _wanderTarget.normalized;
                _nextChangeTime = Time.time + Random.Range(0.5f, 1.5f);
            }
        }

        return _wanderTarget * fish.CommonFishConfig.WanderWeight;
    }
}
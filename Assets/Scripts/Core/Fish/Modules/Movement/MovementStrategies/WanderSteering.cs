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

        // 2. АКТИВНАЯ ЗАЩИТА: Если мы коснулись края, мгновенно "отбиваем" желание плыть в стену
        if (fish.Movement.IsNearEdge)
        {
            var viewportPos = Camera.main.WorldToViewportPoint(fish.transform.position);
            var margin = fish.BoidsConfig.EdgeMargin;
            bool desireChanged = false;

            // Если у левого края и хочет плыть влево -> разворачиваем желание вправо
            if (viewportPos.x < margin && _wanderTarget.x < 0)
            {
                _wanderTarget.x *= -1;
                desireChanged = true;
            }
            // Если у правого края и хочет плыть вправо -> разворачиваем влево
            else if (viewportPos.x > 1f - margin && _wanderTarget.x > 0)
            {
                _wanderTarget.x *= -1;
                desireChanged = true;
            }

            // Если у нижнего края и хочет плыть вниз -> разворачиваем вверх
            if (viewportPos.y < margin && _wanderTarget.y < 0)
            {
                _wanderTarget.y *= -1;
                desireChanged = true;
            }
            // Если у верхнего края и хочет плыть вверх -> разворачиваем вниз
            else if (viewportPos.y > 1f - margin && _wanderTarget.y > 0)
            {
                _wanderTarget.y *= -1;
                desireChanged = true;
            }

            // Если мы скорректировали курс, нормализуем вектор и сбрасываем таймер, 
            // чтобы рыбка успела отплыть по новой траектории
            if (desireChanged)
            {
                _wanderTarget = _wanderTarget.normalized;
                _nextChangeTime = Time.time + Random.Range(1.5f, 3f);
            }
        }

        return _wanderTarget * fish.BoidsConfig.WanderWeight;
    }
}
using Core.Fish.BoidStrategies;
using UnityEngine;

public class BoidsSteering : ISteeringBehavior
{
    private static readonly Collider2D[] _overlapResults = new Collider2D[64]; // В большом аквариуме может переполниться
    
    private readonly ContactFilter2D _filter;

    public BoidsSteering()
    {
        _filter = new ContactFilter2D().NoFilter();
    }

    public Vector2 CalculateSteering(FishEntity fish)
    {
        var alignment = Vector2.zero;
        var cohesion = Vector2.zero;
        var separation = Vector2.zero;
        var neighborCount = 0;
        var separationCount = 0;

        var count = Physics2D.OverlapCircle(fish.transform.position, fish.CommonFishConfig.NeighborRadius, _filter, _overlapResults);

        for (var i = 0; i < count; i++)
        {
            var col = _overlapResults[i];

            if (col.gameObject == fish.gameObject) continue;

            if (!fish.Movement.FishesCache.TryGetValue(col, out var otherFish))
            {
                //Debug.LogWarning($"Не удалось найти конфиг рыбы {count} в кеше");
                continue;
            }

            if (!otherFish.IsAlive) continue;
            
            neighborCount++;
            alignment += otherFish.Movement.Velocity;
            cohesion += (Vector2)otherFish.transform.position;

            var distance = Vector2.Distance(fish.transform.position, otherFish.transform.position);

            if (distance < fish.CommonFishConfig.SeparationRadius && distance > 0)
            {
                var diff = (Vector2)fish.transform.position - (Vector2)otherFish.transform.position;
                var repulsionForce = 1f - (distance / fish.CommonFishConfig.SeparationRadius);
                
                separation += diff.normalized * repulsionForce;
                separationCount++;
            }
        }

        if (neighborCount > 0)
        {
            alignment = (alignment / neighborCount).normalized * fish.CommonFishConfig.AlignmentWeight;
            cohesion = ((cohesion / neighborCount) - (Vector2)fish.transform.position).normalized * fish.CommonFishConfig.CohesionWeight;
        }

        if (separationCount > 0)
        {
            separation = (separation / separationCount) * fish.CommonFishConfig.SeparationWeight;
        }

        return alignment + cohesion + separation;
    }
}
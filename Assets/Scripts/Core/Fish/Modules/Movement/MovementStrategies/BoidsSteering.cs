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

        var count = Physics2D.OverlapCircle(fish.transform.position, fish.Config.NeighborRadius, _filter, _overlapResults);

        for (var i = 0; i < count; i++)
        {
            var col = _overlapResults[i];

            if (col.gameObject == fish.gameObject) continue;
            if (!fish.Movement.FishesCache.TryGetValue(col, out var otherFish)) continue;

            neighborCount++;
            alignment += otherFish.Movement.Velocity;
            cohesion += (Vector2)otherFish.transform.position;

            var distance = Vector2.Distance(fish.transform.position, otherFish.transform.position);

            if (distance < fish.Config.SeparationRadius && distance > 0)
            {
                var diff = (Vector2)fish.transform.position - (Vector2)otherFish.transform.position;
                
                separation += diff.normalized / distance;
            }
        }

        if (neighborCount > 0)
        {
            alignment = (alignment / neighborCount).normalized * fish.Config.AlignmentWeight;
            cohesion = ((cohesion / neighborCount) - (Vector2)fish.transform.position).normalized * fish.Config.CohesionWeight;
            separation = (separation / neighborCount).normalized * fish.Config.SeparationWeight;
        }

        return alignment + cohesion + separation;
    }
}
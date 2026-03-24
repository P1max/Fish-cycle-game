using Core.Fish.BoidStrategies;
using UnityEngine;

public class BoidsSteering : ISteeringBehavior
{
    public Vector2 CalculateSteering(FishEntity fish)
    {
        var alignment = Vector2.zero;
        var cohesion = Vector2.zero;
        var separation = Vector2.zero;
        var neighborCount = 0;
        var separationCount = 0;

        foreach (var otherFish in fish.Scanner.NearbyAliveFishes)
        {
            var distance = Vector2.Distance(fish.transform.position, otherFish.transform.position);

            if (distance > fish.CommonFishConfig.NeighborRadius) continue;

            neighborCount++;
            alignment += otherFish.Movement.Velocity;
            cohesion += (Vector2)otherFish.transform.position;

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
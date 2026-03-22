using Core.Fish.BoidStrategies;
using UnityEngine;

public class BoundsSteering : ISteeringBehavior
{
    private readonly Camera _camera;

    public BoundsSteering(Camera camera)
    {
        _camera = camera;
    }

    public Vector2 CalculateSteering(FishEntity fish)
    {
        var viewportPos = _camera.WorldToViewportPoint(fish.transform.position);
        var boundsSteer = Vector2.zero;
        var margin = fish.BoidsConfig.EdgeMargin;

        fish.Movement.IsNearEdge = false;

        if (viewportPos.x < margin)
        {
            var depth = 1f - (viewportPos.x / margin); 
            
            boundsSteer.x = depth;
            fish.Movement.IsNearEdge = true;
        }
        else if (viewportPos.x > 1f - margin)
        {
            var depth = (viewportPos.x - (1f - margin)) / margin; 
            
            boundsSteer.x = -depth;
            fish.Movement.IsNearEdge = true;
        }

        if (viewportPos.y < margin)
        {
            var depth = 1f - (viewportPos.y / margin);
            
            boundsSteer.y = depth;
            fish.Movement.IsNearEdge = true;
        }
        else if (viewportPos.y > 1f - margin)
        {
            var depth = (viewportPos.y - (1f - margin)) / margin;
            
            boundsSteer.y = -depth;
            fish.Movement.IsNearEdge = true;
        }

        return boundsSteer * fish.BoidsConfig.BoundsWeight;
    }
}
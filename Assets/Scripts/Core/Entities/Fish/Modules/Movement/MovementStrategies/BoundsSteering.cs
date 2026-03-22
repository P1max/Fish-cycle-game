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

        fish.Movement.IsNearEdge = false;
        
        var marginX = fish.CommonFishConfig.MarginXSides;
        var marginTop = fish.CommonFishConfig.MarginTop;
        var marginBottom = fish.CommonFishConfig.MarginBottom;

        if (viewportPos.x < marginX)
        {
            var depth = 1f - (viewportPos.x / marginX); 
            boundsSteer.x = depth;
            fish.Movement.IsNearEdge = true;
        }
        else if (viewportPos.x > 1f - marginX)
        {
            var depth = (viewportPos.x - (1f - marginX)) / marginX; 
            boundsSteer.x = -depth;
            fish.Movement.IsNearEdge = true;
        }

        if (viewportPos.y < marginBottom)
        {
            var depth = 1f - (viewportPos.y / marginBottom);
            boundsSteer.y = depth;
            fish.Movement.IsNearEdge = true;
        }
        else if (viewportPos.y > 1f - marginTop)
        {
            var depth = (viewportPos.y - (1f - marginTop)) / marginTop;
            boundsSteer.y = -depth;
            fish.Movement.IsNearEdge = true;
        }

        return boundsSteer * fish.CommonFishConfig.BoundsWeight;
    }
}
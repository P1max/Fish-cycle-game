using Core.Fish.BoidStrategies;
using Core.Game;
using UnityEngine;

public class BoundsSteering : ISteeringBehavior
{
    private readonly AquariumBoundsManager _boundsManager;

    public BoundsSteering(AquariumBoundsManager boundsManager)
    {
        _boundsManager = boundsManager;
    }

    public Vector2 CalculateSteering(FishEntity fish)
    {
        var bounds = _boundsManager.WorldBounds;
        var pos = fish.transform.position;
        var boundsSteer = Vector2.zero;

        fish.Movement.IsNearEdge = false;

        var marginX = bounds.width * fish.CommonFishConfig.MarginXSides;
        var marginTop = bounds.height * fish.CommonFishConfig.MarginTop;
        var marginBottom = bounds.height * fish.CommonFishConfig.MarginBottom;

        if (pos.x < bounds.xMin + marginX)
        {
            var depth = 1f - ((pos.x - bounds.xMin) / marginX);
            
            boundsSteer.x = depth;
            fish.Movement.IsNearEdge = true;
        }
        else if (pos.x > bounds.xMax - marginX)
        {
            var depth = ((pos.x - (bounds.xMax - marginX)) / marginX);

            boundsSteer.x = -depth;
            fish.Movement.IsNearEdge = true;
        }

        if (pos.y < bounds.yMin + marginBottom)
        {
            var depth = 1f - ((pos.y - bounds.yMin) / marginBottom);
            
            boundsSteer.y = depth;
            fish.Movement.IsNearEdge = true;
        }
        else if (pos.y > bounds.yMax - marginTop)
        {
            var depth = ((pos.y - (bounds.yMax - marginTop)) / marginTop);
            
            boundsSteer.y = -depth;
            fish.Movement.IsNearEdge = true;
        }

        return boundsSteer * fish.CommonFishConfig.BoundsWeight;
    }
}
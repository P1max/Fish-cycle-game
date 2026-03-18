using Core.Fish.BoidStrategies;
using UnityEngine;

public class BoundsSteering : ISteeringBehavior
{
    private readonly Camera _camera;

    public BoundsSteering(Camera camera)
    {
        _camera = camera;
    }

    public Vector2 CalculateSteering(FishMovement fish)
    {
        var viewportPos = _camera.WorldToViewportPoint(fish.transform.position);
        var boundsSteer = Vector2.zero;

        fish.IsNearEdge = false;

        if (viewportPos.x < fish.Config.EdgeMargin) { boundsSteer.x = 1f; fish.IsNearEdge = true; }
        else if (viewportPos.x > 1f - fish.Config.EdgeMargin) { boundsSteer.x = -1f; fish.IsNearEdge = true; }

        if (viewportPos.y < fish.Config.EdgeMargin) { boundsSteer.y = 1f; fish.IsNearEdge = true; }
        else if (viewportPos.y > 1f - fish.Config.EdgeMargin) { boundsSteer.y = -1f; fish.IsNearEdge = true; }

        return boundsSteer.normalized * fish.Config.BoundsWeight;
    }
}
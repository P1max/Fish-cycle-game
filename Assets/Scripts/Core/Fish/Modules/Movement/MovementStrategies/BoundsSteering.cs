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

        if (viewportPos.x < fish.Config.EdgeMargin) { boundsSteer.x = 1f; fish.Movement.IsNearEdge = true; }
        else if (viewportPos.x > 1f - fish.Config.EdgeMargin) { boundsSteer.x = -1f; fish.Movement.IsNearEdge = true; }

        if (viewportPos.y < fish.Config.EdgeMargin) { boundsSteer.y = 1f; fish.Movement.IsNearEdge = true; }
        else if (viewportPos.y > 1f - fish.Config.EdgeMargin) { boundsSteer.y = -1f; fish.Movement.IsNearEdge = true; }

        return boundsSteer.normalized * fish.Config.BoundsWeight;
    }
}
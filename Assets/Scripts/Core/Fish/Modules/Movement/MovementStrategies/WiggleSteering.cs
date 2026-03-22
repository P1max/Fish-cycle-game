using Core.Fish.BoidStrategies;
using UnityEngine;

public class WiggleSteering : ISteeringBehavior
{
    private readonly float _wiggleOffset;
    private readonly float _wiggleFrequency = 3f;
    private readonly float _wiggleForce = 10f;

    public WiggleSteering()
    {
        _wiggleOffset = Random.Range(0f, 100f);
    }

    public Vector2 CalculateSteering(FishEntity fish)
    {
        var velocity = fish.Movement.Velocity;
        
        if (velocity.sqrMagnitude < 0.1f) return Vector2.zero;

        var perpendicular = new Vector2(-velocity.y, velocity.x).normalized;

        var wiggleAmount = Mathf.Sin((Time.time + _wiggleOffset) * _wiggleFrequency) * _wiggleForce;

        return perpendicular * wiggleAmount;
    }
}
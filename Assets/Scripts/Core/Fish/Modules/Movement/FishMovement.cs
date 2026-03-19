using System.Collections.Generic;
using Core.Fish.BoidStrategies;
using Spawners;
using UnityEngine;

public class FishMovement
{
    private ISteeringBehavior[] _behaviors;
    private FishEntity _fishEntity;
    private Rigidbody2D _rigidbody;
    private Vector2 _velocity;
    private bool _isActive;

    public IReadOnlyDictionary<Collider2D, FishEntity> FishesCache { get; private set; }
    public bool IsNearEdge { get; set; }
    public Vector2 Velocity => _velocity;

    public FishMovement(FishEntity fishEntity, IReadOnlyDictionary<Collider2D, FishEntity> fishesCache, FoodPool foodPool,
        Rigidbody2D rigidbody)
    {
        _fishEntity = fishEntity;
        FishesCache = fishesCache;
        _rigidbody = rigidbody;

        _behaviors = new ISteeringBehavior[]
        {
            new WanderSteering(),
            new BoundsSteering(Camera.main),
            new BoidsSteering(),
            new FoodSteering(foodPool)
        };

        _isActive = true;
    }

    public void Stop()
    {
        _isActive = false;

        _rigidbody.linearVelocity = Vector2.zero;
    }

    public void Tick()
    {
        if (!_fishEntity.Config || !_isActive || _behaviors == null) return;

        var acceleration = Vector2.zero;

        foreach (var behavior in _behaviors)
            acceleration += behavior.CalculateSteering(_fishEntity);

        _velocity += acceleration * Time.fixedDeltaTime;

        if (IsNearEdge)
            _velocity = Vector2.Lerp(_velocity, _velocity.normalized * _fishEntity.Config.SpeedRange.x, Time.fixedDeltaTime * 3f);

        var speed = _velocity.magnitude;

        var hungerMultiplier = 1f + (_fishEntity.Hunger.CurrentHungerPercent / 100f * 0.5f);
        var currentMaxSpeed = _fishEntity.Config.SpeedRange.y * hungerMultiplier;

        if (speed > currentMaxSpeed) _velocity = _velocity.normalized * currentMaxSpeed;
        else if (speed < _fishEntity.Config.SpeedRange.x) _velocity = _velocity.normalized * _fishEntity.Config.SpeedRange.x;

        _rigidbody.linearVelocity = _velocity;

        _fishEntity.FishVisual.UpdateVisuals();
    }
}
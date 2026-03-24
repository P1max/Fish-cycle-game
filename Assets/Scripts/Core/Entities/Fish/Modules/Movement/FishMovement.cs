using System.Collections.Generic;
using Core.Fish.BoidStrategies;
using Core.Game;
using Spawners;
using UnityEngine;

public class FishMovement
{
    private ISteeringBehavior[] _behaviors;
    private FishEntity _fishEntity;
    private Rigidbody2D _rigidbody;
    private Vector2 _velocity;
    private bool _isActive;
    private float _currentSpeedLimit;

    public IReadOnlyDictionary<Collider2D, FishEntity> FishesCache { get; private set; }
    public bool IsNearEdge { get; set; }
    public bool IsChasingFood { get; set; }
    public Vector2 Velocity => _velocity;

    public FishMovement(FishEntity fishEntity, IReadOnlyDictionary<Collider2D, FishEntity> fishesCache, FoodPool foodPool,
        Rigidbody2D rigidbody, Collider2D collider, AquariumBoundsManager aquariumBoundsManager, CoinsPool coinsPool)
    {
        _fishEntity = fishEntity;
        FishesCache = fishesCache;
        _rigidbody = rigidbody;

        _behaviors = new ISteeringBehavior[]
        {
            new BoundsSteering(aquariumBoundsManager),
            new ObstacleSteering(coinsPool),
            new WanderSteering(),
            new BoidsSteering(),
            new FoodSteering(foodPool, collider),
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

        var targetSpeedLimit = IsChasingFood ? _fishEntity.Config.MaxHungrySpeed : _fishEntity.BaseSpeed;
        
        _currentSpeedLimit = Mathf.Lerp(_currentSpeedLimit, targetSpeedLimit, Time.fixedDeltaTime * 3f);

        _velocity = Vector2.ClampMagnitude(_velocity, _currentSpeedLimit);

        var speed = _velocity.magnitude;

        if (!IsChasingFood && speed < _fishEntity.BaseSpeed && speed > 0.1f && !IsNearEdge)
        {
            _velocity = Vector2.Lerp(_velocity, _velocity.normalized * _fishEntity.BaseSpeed, Time.fixedDeltaTime * 2f);
        }

        _rigidbody.linearVelocity = _velocity;

        _fishEntity.FishVisual.UpdateVisuals();
    }
}
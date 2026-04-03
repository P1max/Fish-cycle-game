using Core.Fish.BoidStrategies;
using Core.Game;
using UI.FeedJar;
using UnityEngine;

public class FishMovement
{
    private ISteeringBehavior[] _behaviors;
    private FishEntity _fishEntity;
    private Rigidbody2D _rigidbody;
    private Vector2 _velocity;
    private bool _isActive;
    private float _currentSpeedLimit;

    public bool IsNearEdge { get; set; }
    public bool IsChasingFood { get; set; }
    public Vector2 Velocity => _velocity;

    public Vector2 ScareSource { get; private set; }
    public float ScareTimer { get; private set; }
    public bool IsScared => ScareTimer > 0;

    public FishMovement(FishEntity fishEntity, Rigidbody2D rigidbody, Collider2D collider, AquariumBoundsManager aquariumBoundsManager
    , FeedJarView feedJarView)
    {
        _fishEntity = fishEntity;
        _rigidbody = rigidbody;

        _behaviors = new ISteeringBehavior[]
        {
            new BoundsSteering(aquariumBoundsManager),
            new ObstacleSteering(feedJarView),
            new WanderSteering(),
            new BoidsSteering(),
            new FoodSteering(collider),
            new FleeSteering()
        };

        _isActive = true;
    }

    public void Start()
    {
        _isActive = true;
    }

    public void Stop()
    {
        _isActive = false;

        _rigidbody.linearVelocity = Vector2.zero;
    }

    public void ApplyScare(Vector2 sourcePosition, float duration)
    {
        ScareSource = sourcePosition;
        ScareTimer = duration;
    }

    public void Tick()
    {
        if (_fishEntity.Config == null || !_isActive || _behaviors == null) return;

        if (ScareTimer > 0) ScareTimer -= Time.fixedDeltaTime;

        var acceleration = Vector2.zero;

        foreach (var behavior in _behaviors)
            acceleration += behavior.CalculateSteering(_fishEntity);

        _velocity += acceleration * Time.fixedDeltaTime;

        var targetSpeedLimit = IsScared
            ? _fishEntity.Config.MaxHungrySpeed * 1.5f
            : (IsChasingFood ? _fishEntity.Config.MaxHungrySpeed : _fishEntity.BaseSpeed);

        var lerpSpeed = IsScared ? 5f : 3f;

        _currentSpeedLimit = Mathf.Lerp(_currentSpeedLimit, targetSpeedLimit, Time.fixedDeltaTime * lerpSpeed);

        _velocity = Vector2.ClampMagnitude(_velocity, _currentSpeedLimit);

        var speed = _velocity.magnitude;

        if (!IsChasingFood && !IsScared && speed < _fishEntity.BaseSpeed && speed > 0.1f && !IsNearEdge) 
            _velocity = Vector2.Lerp(_velocity, _velocity.normalized * _fishEntity.BaseSpeed, Time.fixedDeltaTime * 2f);

        _rigidbody.linearVelocity = _velocity;
    }
}
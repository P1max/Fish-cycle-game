using System.Collections.Generic;
using Core.Fish.BoidStrategies;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class FishMovement : MonoBehaviour
{
    [SerializeField] private Transform _visualTransform;

    private ISteeringBehavior[] _behaviors;
    private FishEntity _fishEntity;
    private FishConfig _config;
    private Rigidbody2D _rigidbody;
    private Vector2 _velocity;
    private bool _isActive;

    public IReadOnlyDictionary<Collider2D, FishEntity> FishesCache { get; private set; }
    public bool IsNearEdge { get; set; }
    public FishConfig Config => _config;
    public Vector2 Velocity => _velocity;

    private void OnEnable() => _isActive = true;

    private void OnDisable() => _isActive = false;

    private void Awake() => _rigidbody = GetComponent<Rigidbody2D>();

    private void FixedUpdate()
    {
        if (!_config || !_isActive || _behaviors == null) return;

        var acceleration = Vector2.zero;

        foreach (var behavior in _behaviors)
            acceleration += behavior.CalculateSteering(this);

        _velocity += acceleration * Time.fixedDeltaTime;

        if (IsNearEdge)
            _velocity = Vector2.Lerp(_velocity, _velocity.normalized * _config.SpeedRange.x, Time.fixedDeltaTime * 3f);

        var speed = _velocity.magnitude;

        if (speed > _config.SpeedRange.y) _velocity = _velocity.normalized * _config.SpeedRange.y;
        else if (speed < _config.SpeedRange.x) _velocity = _velocity.normalized * _config.SpeedRange.x;

        _rigidbody.linearVelocity = _velocity;

        UpdateVisuals();
    }

    private void UpdateVisuals()
    {
        if (_velocity.x > 0.3f)
            _visualTransform.localScale = new Vector3(-1 * Mathf.Abs(_visualTransform.localScale.x), _visualTransform.localScale.y,
                _visualTransform.localScale.z);
        else if (_velocity.x < -0.3f)
            _visualTransform.localScale = new Vector3(1 * Mathf.Abs(_visualTransform.localScale.x), _visualTransform.localScale.y,
                _visualTransform.localScale.z);

        var targetAngleZ = Mathf.Clamp((_velocity.y / _config.SpeedRange.y) * _config.MaxTiltAngle, -_config.MaxTiltAngle,
            _config.MaxTiltAngle);

        var tiltDirection = -_visualTransform.localScale.x;
        var targetRotation = Quaternion.Euler(0, 0, targetAngleZ * tiltDirection);

        _visualTransform.localRotation = Quaternion.Slerp(
            _visualTransform.localRotation,
            targetRotation,
            _config.SteerSpeed * Time.fixedDeltaTime
        );
    }

    public void Init(FishEntity fishEntity, IReadOnlyDictionary<Collider2D, FishEntity> fishesCache)
    {
        _fishEntity = fishEntity;
        FishesCache = fishesCache;
        _config = _fishEntity.Config;

        if (!_config) Debug.LogWarning("Рыбе не назначен конфиг.");

        var initialSpeed = Random.Range(_config.SpeedRange.x, _config.SpeedRange.y);

        _velocity = Random.insideUnitCircle.normalized * initialSpeed;

        _behaviors = new ISteeringBehavior[]
        {
            new WanderSteering(),
            new BoundsSteering(Camera.main),
            new BoidsSteering()
        };
    }
}
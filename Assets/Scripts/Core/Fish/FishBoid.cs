using System.Collections.Generic;
using Core.Fish.BoidStrategies;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class FishBoid : MonoBehaviour
{
    [SerializeField] private FishConfig _config;
    [SerializeField] private Transform _visualTransform;

    public FishConfig Config => _config;
    public Vector2 Velocity => _velocity;
    public IReadOnlyDictionary<Collider2D, FishBoid> FishesCache { get; private set; }
    public bool IsNearEdge { get; set; }

    private Rigidbody2D _rigidbody;
    private Vector2 _velocity;
    private bool _isActive;

    private ISteeringBehavior[] _behaviors;

    private void Awake()
    {
        if (!_config) Debug.LogWarning("Рыбе не назначен конфиг.", gameObject);

        _rigidbody = GetComponent<Rigidbody2D>();
    }

    public void Init(IReadOnlyDictionary<Collider2D, FishBoid> fishesCache)
    {
        FishesCache = fishesCache;

        var initialSpeed = Random.Range(_config.SpeedRange.x, _config.SpeedRange.y);
        _velocity = Random.insideUnitCircle.normalized * initialSpeed;

        _behaviors = new ISteeringBehavior[]
        {
            new WanderSteering(),
            new BoundsSteering(Camera.main),
            new BoidsSteering()
        };
    }

    private void OnEnable() => _isActive = true;
    private void OnDisable() => _isActive = false;

    private void FixedUpdate()
    {
        if (!_config || !_isActive || _behaviors == null) return;

        var acceleration = Vector2.zero;

        // Магия OCP: рыбке всё равно, сколько и каких поведений на ней висит!
        foreach (var behavior in _behaviors)
        {
            acceleration += behavior.CalculateSteering(this);
        }

        _velocity += acceleration * Time.fixedDeltaTime;

        // Торможение у краев
        if (IsNearEdge)
            _velocity = Vector2.Lerp(_velocity, _velocity.normalized * _config.SpeedRange.x, Time.fixedDeltaTime * 3f);

        // Ограничение скорости
        var speed = _velocity.magnitude;

        if (speed > _config.SpeedRange.y) _velocity = _velocity.normalized * _config.SpeedRange.y;
        else if (speed < _config.SpeedRange.x) _velocity = _velocity.normalized * _config.SpeedRange.x;

        _rigidbody.linearVelocity = _velocity;

        UpdateVisuals();
    }

    private void UpdateVisuals()
    {
        if (_velocity.x > 0.3f)
            _visualTransform.localScale = new Vector3(-1, _visualTransform.localScale.y, _visualTransform.localScale.z);
        else if (_velocity.x < -0.3f)
            _visualTransform.localScale = new Vector3(1, _visualTransform.localScale.y, _visualTransform.localScale.z);

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
}
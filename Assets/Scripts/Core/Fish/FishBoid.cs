using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class FishBoid : MonoBehaviour
{
    [SerializeField] private FishConfig _config;
    [SerializeField] private Transform _visualTransform;

    private IReadOnlyDictionary<Collider2D, FishBoid> _fishesCache;

    private Rigidbody2D _rigidbody;
    private Camera _mainCamera;
    private Vector2 _wanderTarget;
    private Vector2 _velocity;
    private bool _isNearEdge;
    private bool _isActive;

    private Coroutine _coroutine;

    private void Start()
    {
        if (!_config)
        {
            Debug.LogWarning("Рыбе не назначен конфиг.");

            return;
        }

        var initialSpeed = Random.Range(_config.SpeedRange.x, _config.SpeedRange.y);

        _mainCamera = Camera.main;
        _rigidbody = GetComponent<Rigidbody2D>();
        _velocity = Random.insideUnitCircle.normalized * initialSpeed;
    }

    private void OnEnable()
    {
        _isActive = true;

        if (_coroutine != null) StopCoroutine(_coroutine);

        _coroutine = StartCoroutine(WanderRoutine());
    }

    private void OnDisable()
    {
        if (_coroutine != null) StopCoroutine(_coroutine);
        
        _isActive = false;
    }

    private IEnumerator WanderRoutine()
    {
        while (_isActive)
        {
            _wanderTarget = Random.insideUnitCircle.normalized;

            yield return new WaitForSeconds(2f + Random.Range(0f, 2f));
        }

        yield return null;
    }

    private void FixedUpdate()
    {
        if (!_config || !_isActive) return;

        var acceleration = Vector2.zero;
        acceleration += CalculateBoidsAcceleration();
        acceleration += CalculateBoundsAcceleration();
        acceleration += CalculateWanderAcceleration();

        // TODO acceleration += CalculateFoodAcceleration();

        _velocity += acceleration * Time.fixedDeltaTime;

        if (_isNearEdge) _velocity = Vector2.Lerp(_velocity, _velocity.normalized * _config.SpeedRange.x, Time.fixedDeltaTime * 3f);

        var speed = _velocity.magnitude;

        if (speed > _config.SpeedRange.y) _velocity = _velocity.normalized * _config.SpeedRange.y;
        else if (speed < _config.SpeedRange.x) _velocity = _velocity.normalized * _config.SpeedRange.x;

        _rigidbody.linearVelocity = _velocity;

        UpdateVisuals();

        return;

        Vector2 CalculateWanderAcceleration()
        {
            return _wanderTarget * _config.WanderWeight;
        }

        Vector2 CalculateBoundsAcceleration()
        {
            var viewportPos = _mainCamera.WorldToViewportPoint(transform.position);
            var boundsSteer = Vector2.zero;

            _isNearEdge = false;

            if (viewportPos.x < _config.EdgeMargin)
            {
                boundsSteer.x = 1f;
                _isNearEdge = true;
            }
            else if (viewportPos.x > 1f - _config.EdgeMargin)
            {
                boundsSteer.x = -1f;
                _isNearEdge = true;
            }

            if (viewportPos.y < _config.EdgeMargin)
            {
                boundsSteer.y = 1f;
                _isNearEdge = true;
            }
            else if (viewportPos.y > 1f - _config.EdgeMargin)
            {
                boundsSteer.y = -1f;
                _isNearEdge = true;
            }

            if (_isNearEdge) _wanderTarget = boundsSteer.normalized;

            return boundsSteer.normalized * _config.BoundsWeight;
        }

        Vector2 CalculateBoidsAcceleration()
        {
            var alignment = Vector2.zero;
            var cohesion = Vector2.zero;
            var separation = Vector2.zero;
            var neighborCount = 0;

            var colliders = Physics2D.OverlapCircleAll(transform.position, _config.NeighborRadius);

            foreach (var col in colliders)
            {
                if (col.gameObject == gameObject) continue;
                if (!col.TryGetComponent<FishBoid>(out var otherFish)) continue;

                neighborCount++;
                alignment += otherFish._velocity;
                cohesion += (Vector2)otherFish.transform.position;

                var distance = Vector2.Distance(transform.position, otherFish.transform.position);

                if (distance < _config.SeparationRadius && distance > 0)
                {
                    var diff = (Vector2)transform.position - (Vector2)otherFish.transform.position;
                    separation += diff.normalized / distance;
                }
            }

            if (neighborCount > 0)
            {
                alignment = (alignment / neighborCount).normalized * _config.AlignmentWeight;
                cohesion = ((cohesion / neighborCount) - (Vector2)transform.position).normalized * _config.CohesionWeight;
                separation = (separation / neighborCount).normalized * _config.SeparationWeight;
            }

            return alignment + cohesion + separation;
        }

        void UpdateVisuals()
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

    public void Init(IReadOnlyDictionary<Collider2D, FishBoid> fishesCache)
    {
        _fishesCache = fishesCache;
    }
}
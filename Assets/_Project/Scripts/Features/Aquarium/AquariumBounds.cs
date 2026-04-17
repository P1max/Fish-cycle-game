using System;
using _Project.Core.Interfaces;
using Core.Game;
using UnityEngine;
using Zenject;

[RequireComponent(typeof(EdgeCollider2D))]
public class AquariumBounds : MonoBehaviour, IGameplayInit, IDisposable
{
    [Inject] private AquariumBoundsManager _boundsManager;

    [SerializeField] private float _thickness = 1f;

    private EdgeCollider2D _edgeCollider;
    private bool _isInit;

    private void Awake()
    {
        _edgeCollider = GetComponent<EdgeCollider2D>();
    }

    private void BuildCollider()
    {
        if (!_isInit) return;

        var bounds = _boundsManager.WorldBounds;

        var bottomLeft = new Vector2(bounds.xMin - _thickness, bounds.yMin - _thickness);
        var topLeft = new Vector2(bounds.xMin - _thickness, bounds.yMax + _thickness);
        var topRight = new Vector2(bounds.xMax + _thickness, bounds.yMax + _thickness);
        var bottomRight = new Vector2(bounds.xMax + _thickness, bounds.yMin - _thickness);

        _edgeCollider.points = new[] { bottomLeft, topLeft, topRight, bottomRight, bottomLeft };

        _edgeCollider.edgeRadius = _thickness;
    }

    public void Init()
    {
        _isInit = true;

        _boundsManager.OnBoundsUpdated += BuildCollider;

        if (_boundsManager.WorldBounds.width > 0) BuildCollider();
    }

    public void Dispose()
    {
        _boundsManager.OnBoundsUpdated -= BuildCollider;
    }
}
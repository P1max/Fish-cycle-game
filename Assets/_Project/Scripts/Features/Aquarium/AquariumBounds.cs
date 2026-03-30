using Core.Game;
using UnityEngine;
using Zenject;

[RequireComponent(typeof(EdgeCollider2D))]
public class AquariumBounds : MonoBehaviour
{
    [Inject] private AquariumBoundsManager _boundsManager;

    [SerializeField] private float _thickness = 1f;

    private EdgeCollider2D _edgeCollider;

    private void Awake()
    {
        _edgeCollider = GetComponent<EdgeCollider2D>();
    }

    private void Start()
    {
        _boundsManager.OnBoundsUpdated += BuildCollider;

        if (_boundsManager.WorldBounds.width > 0) BuildCollider();
    }

    private void OnDestroy()
    {
        if (_boundsManager != null) _boundsManager.OnBoundsUpdated -= BuildCollider;
    }

    private void BuildCollider()
    {
        var bounds = _boundsManager.WorldBounds;

        var bottomLeft = new Vector2(bounds.xMin - _thickness, bounds.yMin - _thickness);
        var topLeft = new Vector2(bounds.xMin - _thickness, bounds.yMax + _thickness);
        var topRight = new Vector2(bounds.xMax + _thickness, bounds.yMax + _thickness);
        var bottomRight = new Vector2(bounds.xMax + _thickness, bounds.yMin - _thickness);

        _edgeCollider.points = new[] { bottomLeft, topLeft, topRight, bottomRight, bottomLeft };

        _edgeCollider.edgeRadius = _thickness;
    }
}
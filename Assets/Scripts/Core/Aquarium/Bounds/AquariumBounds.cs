using Core.Game;
using UnityEngine;
using Zenject;

[RequireComponent(typeof(EdgeCollider2D))]
public class AquariumBounds : MonoBehaviour
{
    [Inject] private AquariumBoundsManager _boundsManager;
    
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

        var bottomLeft = new Vector2(bounds.xMin, bounds.yMin);
        var topLeft = new Vector2(bounds.xMin, bounds.yMax);
        var topRight = new Vector2(bounds.xMax, bounds.yMax);
        var bottomRight = new Vector2(bounds.xMax, bounds.yMin);

        _edgeCollider.points = new[] { bottomLeft, topLeft, topRight, bottomRight, bottomLeft };
    }
}
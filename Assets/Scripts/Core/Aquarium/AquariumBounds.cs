using UnityEngine;

public class AquariumBounds : MonoBehaviour
{
    private void Awake()
    {
        CreateScreenBounds();
    }

    private void CreateScreenBounds()
    {
        var cam = Camera.main;

        if (cam == null) return;

        var bottomLeft = (Vector2)cam.ViewportToWorldPoint(new Vector3(0, 0, cam.nearClipPlane));
        var topRight = (Vector2)cam.ViewportToWorldPoint(new Vector3(1, 1, cam.nearClipPlane));
        var topLeft = new Vector2(bottomLeft.x, topRight.y);
        var bottomRight = new Vector2(topRight.x, bottomLeft.y);

        var edgeCollider = gameObject.AddComponent<EdgeCollider2D>();

        edgeCollider.points = new[] { bottomLeft, topLeft, topRight, bottomRight, bottomLeft };
    }
}
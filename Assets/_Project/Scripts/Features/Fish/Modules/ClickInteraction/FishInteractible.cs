using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Collider2D))]
public class FishInteractable : MonoBehaviour
{
    private FishEntity _fishEntity;

    private void Awake()
    {
        _fishEntity = GetComponent<FishEntity>();
    }

    private void OnMouseDown()
    {
#if UNITY_ANDROID || UNITY_IOS
        if (Input.touchCount > 0 && EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId)) return;
#else
        if (EventSystem.current.IsPointerOverGameObject()) return;
#endif

        if (!_fishEntity.IsAlive) _fishEntity.Collect();
    }
}
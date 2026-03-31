using _Project.Core.Interfaces;
using Spawners;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

public class WaterInteractionManager : MonoBehaviour, IGameplayInit
{
    [Inject] private FishPool _fishPool;
    [Inject] private CommonFishConfig _commonFishConfig;
    [Inject] private EffectsPool _effectsPool;

    private bool _isInit;

    private void Update()
    {
        if (!_isInit) return;

        if (Input.GetMouseButtonDown(0)) HandleWaterClick();
    }

    private void HandleWaterClick()
    {
#if UNITY_ANDROID || UNITY_IOS
        if (Input.touchCount > 0 && EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId)) return;
#else
        if (EventSystem.current.IsPointerOverGameObject()) return;
#endif

        Vector2 clickPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        var hitCollider = Physics2D.OverlapPoint(clickPosition);

        if (hitCollider != null)
            return;

        _effectsPool.SpawnEffect("bubbles", clickPosition);

        var allFishes = _fishPool.ActiveFishes;

        foreach (var fish in allFishes)
        {
            if (fish.IsAlive)
            {
                var distance = Vector2.Distance(clickPosition, fish.transform.position);

                if (distance <= _commonFishConfig.ScareRadius)
                {
                    fish.Movement.ApplyScare(clickPosition, _commonFishConfig.ScareDuration);
                }
            }
        }
    }

    public void Init()
    {
        _isInit = true;
    }
}
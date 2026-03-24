using UnityEngine;
using UnityEngine.UI;

[ExecuteAlways]
[RequireComponent(typeof(CanvasScaler))]
public class DynamicCanvasScaler : MonoBehaviour
{
    private CanvasScaler _canvasScaler;
    private float _lastScreenWidth;
    private float _lastScreenHeight;

    private void Awake()
    {
        _canvasScaler = GetComponent<CanvasScaler>();
        ApplyCorrection();
    }

    private void Update()
    {
        if (Screen.width != _lastScreenWidth || Screen.height != _lastScreenHeight)
        {
            ApplyCorrection();

            _lastScreenWidth = Screen.width;
            _lastScreenHeight = Screen.height;
        }
    }

    private void ApplyCorrection()
    {
        if (_canvasScaler == null || _canvasScaler.uiScaleMode != CanvasScaler.ScaleMode.ScaleWithScreenSize)
            return;

        var screenRatio = (float)Screen.width / Screen.height;

        var targetRatio = _canvasScaler.referenceResolution.x / _canvasScaler.referenceResolution.y;

        _canvasScaler.matchWidthOrHeight = screenRatio < targetRatio ? 0f : 1f;
    }
}
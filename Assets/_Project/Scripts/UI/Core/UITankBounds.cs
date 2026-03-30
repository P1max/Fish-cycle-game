using Core.Game;
using UnityEngine;
using Zenject;

namespace UI
{
    public class UITankBounds : MonoBehaviour
    {
        [Inject] private AquariumBoundsManager _boundsManager;

        private RectTransform _rectTransform;

        private void Awake()
        {
            _rectTransform = GetComponent<RectTransform>();
        }

        private void Start()
        {
            UpdateWorldBounds();
        }

        private void OnRectTransformDimensionsChange()
        {
            if (_boundsManager != null && _rectTransform != null) UpdateWorldBounds();
        }

        private void UpdateWorldBounds()
        {
            var corners = new Vector3[4];

            _rectTransform.GetWorldCorners(corners);

            var width = corners[2].x - corners[0].x;
            var height = corners[1].y - corners[0].y;

            var worldRect = new Rect(corners[0].x, corners[0].y, width, height);

            _boundsManager.SetBounds(worldRect);
        }
    }
}
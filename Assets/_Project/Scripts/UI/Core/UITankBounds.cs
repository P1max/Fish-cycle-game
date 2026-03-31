using _Project.Core.Interfaces;
using Core.Game;
using UnityEngine;
using Zenject;

namespace UI
{
    public class UITankBounds : MonoBehaviour, IGameplayInit
    {
        [Inject] private AquariumBoundsManager _boundsManager;

        private RectTransform _rectTransform;
        private bool _isInit;

        private void Awake()
        {
            _rectTransform = GetComponent<RectTransform>();
        }

        private void OnRectTransformDimensionsChange()
        {
            UpdateWorldBounds();
        }

        private void UpdateWorldBounds()
        {
            if (!_isInit) return;
            
            var corners = new Vector3[4];

            _rectTransform.GetWorldCorners(corners);

            var width = corners[2].x - corners[0].x;
            var height = corners[1].y - corners[0].y;

            var worldRect = new Rect(corners[0].x, corners[0].y, width, height);

            _boundsManager.SetBounds(worldRect);
        }

        public void Init()
        {
            _isInit = true;
            
            UpdateWorldBounds();
        }
    }
}
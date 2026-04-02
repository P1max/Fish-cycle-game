using System;
using System.Collections.Generic;
using _Project.Core.Interfaces;
using UnityEngine;

namespace UI.Conveyor
{
    public class ConveyorView : BaseView, IConveyorMetricsProvider
    {
        [SerializeField] private RectTransform _container;

        private FishItemView _prefab;
        private RectTransform _viewport;
        private List<FishItemView> _views;

        private float _stepDistance;
        private float _itemWidth;
        private float _targetHeight;
        private float _despawnThresholdX;
        private float _speed;
        private float _spacing;

        public event Action<FishItemView> OnItemRerollRequested;
        public event Action<FishItemView> OnItemClicked;

        private void LoadPrefab() => _prefab = Resources.Load<FishItemView>("Prefabs/Entities/FishItem");

        private void Awake()
        {
            if (_prefab == null) LoadPrefab();
        }

        private void Update()
        {
            if (!_isInit || _views.Count == 0) return;

            _container.anchoredPosition += Vector2.right * (_speed * Time.deltaTime);

            CheckCarousel();
        }

        private void CheckCarousel()
        {
            var rightmostView = _views[0];
            var localPosInViewport = _viewport.InverseTransformPoint(rightmostView.transform.position);

            if (localPosInViewport.x > _despawnThresholdX)
            {
                var leftmostView = _views[^1];
                var rect = rightmostView.Rect;
                var leftmostPos = leftmostView.Rect.anchoredPosition.x;

                rect.anchoredPosition = new Vector2(leftmostPos - _stepDistance, rect.anchoredPosition.y);

                OnItemRerollRequested?.Invoke(rightmostView);

                _views.RemoveAt(0);
                _views.Add(rightmostView);
            }
        }

        public void RefreshOffscreenItems()
        {
            if (_views == null || _views.Count == 0) return;

            foreach (var view in _views)
            {
                var localPosInViewport = _viewport.InverseTransformPoint(view.transform.position);
                var rightEdge = localPosInViewport.x + (_itemWidth / 2f);

                if (rightEdge <= _viewport.rect.xMin - 5f)
                    OnItemRerollRequested?.Invoke(view);
            }
        }

        public void UpdateSpacing(float newSpacing)
        {
            _spacing = newSpacing;
            _stepDistance = _itemWidth + _spacing;

            var basePosX = _views[0].Rect.anchoredPosition.x;

            for (var i = 1; i < _views.Count; i++)
            {
                var rect = _views[i].Rect;

                rect.anchoredPosition = new Vector2(basePosX - i * _stepDistance, rect.anchoredPosition.y);
            }
        }

        public bool IsViewVisible(FishItemView view)
        {
            var localPos = _viewport.InverseTransformPoint(view.transform.position);
            var rightEdge = localPos.x + (_itemWidth / 2f);

            return rightEdge > _viewport.rect.xMin;
        }

        public float GetExactStepDistance(float itemSpacing)
        {
            if (!_prefab) LoadPrefab();

            Canvas.ForceUpdateCanvases();

            var viewport = _container.parent.GetComponent<RectTransform>();
            var aspectRatio = _prefab.Rect.rect.width / _prefab.Rect.rect.height;
            var itemWidth = viewport.rect.height * aspectRatio;

            return itemWidth + itemSpacing;
        }

        public int GetMaxItemsNeeded()
        {
            if (!_prefab) LoadPrefab();

            Canvas.ForceUpdateCanvases();

            var viewport = _container.parent.GetComponent<RectTransform>();
            var aspectRatio = _prefab.Rect.rect.width / _prefab.Rect.rect.height;
            var itemWidth = viewport.rect.height * aspectRatio;

            return Mathf.CeilToInt(viewport.rect.width / itemWidth) + 1;
        }

        public void Init(float speed, float spacing)
        {
            _speed = speed;
            _spacing = spacing;
            _views = new List<FishItemView>();
            _viewport = _container.parent.GetComponent<RectTransform>();

            Canvas.ForceUpdateCanvases();

            var prefabRect = _prefab.Rect;
            var aspectRatio = prefabRect.rect.width / prefabRect.rect.height;

            _targetHeight = _viewport.rect.height;
            _itemWidth = _targetHeight * aspectRatio;
            _stepDistance = _itemWidth + _spacing;
            _despawnThresholdX = _viewport.rect.xMax + (_itemWidth / 2f);

            var maxItemsNeeded = Mathf.CeilToInt(_viewport.rect.width / _itemWidth) + 1;
            var startX = _viewport.rect.xMin - (_itemWidth / 2f);

            _container.anchoredPosition = new Vector2(0, _container.anchoredPosition.y);

            for (var i = 0; i < maxItemsNeeded; i++)
            {
                var view = Instantiate(_prefab, _container, false);
                var rect = view.GetComponent<RectTransform>();

                rect.sizeDelta = new Vector2(_itemWidth, _targetHeight);
                rect.anchoredPosition = new Vector2(startX - (i * _stepDistance), 0);

                view.SetButtonAction(isPurchased =>
                {
                    if (!isPurchased) OnItemClicked?.Invoke(view);
                });

                _views.Add(view);

                OnItemRerollRequested?.Invoke(view);
            }

            _isInit = true;
        }
    }
}
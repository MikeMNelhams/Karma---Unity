using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace CustomUI.Scrollbar
{
    [RequireComponent(typeof(RectTransform))]
    public class VerticalScrollbarSelect : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
    {
        [SerializeField] Canvas _parentCanvas;
        [SerializeField] RectTransform _rectTransform;
        [SerializeField] RectTransform _scrollbarBackground;

        float _scrollbarHeight;

        float _minimumHeight;
        float _maximumHeight;

        public delegate void OnDragListener();
        event OnDragListener OnDragEvent;

        public void SetScrollbarHeight(int numberOfViewHolders, float elementHeight, int activeDisplayCount)
        {
            _scrollbarHeight = _scrollbarBackground.rect.height;

            int displayMaxCount = Mathf.Min(numberOfViewHolders, activeDisplayCount);

            float fraction = Mathf.Min(((_scrollbarHeight) / (displayMaxCount * elementHeight)), 1);

            _rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, fraction * _scrollbarHeight);

            float halfScrollbarHeight = _scrollbarHeight / 2.0f;
            float halfScrollSelectHeight = _rectTransform.rect.height / 2.0f;

            _maximumHeight = halfScrollbarHeight - halfScrollSelectHeight;
            _minimumHeight = -halfScrollbarHeight + halfScrollSelectHeight;

            if (fraction == 1)
            {
                _rectTransform.localPosition = new Vector3(_rectTransform.localPosition.x, _maximumHeight);
            }
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (!eventData.dragging) { return; }

            float yFactor = 1.0f / _parentCanvas.scaleFactor;
            float halfScrollbarHeight = _scrollbarHeight / 2.0f;

            float yNew = Mathf.Clamp(_rectTransform.anchoredPosition.y + eventData.delta.y * yFactor, _minimumHeight, _maximumHeight);
            _rectTransform.anchoredPosition = new Vector2(0, yNew);

            OnDragEvent?.Invoke();
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
        }

        public void OnEndDrag(PointerEventData eventData)
        {
        }

        public void RegisterOnDragListener(OnDragListener onDragEndListener)
        {
            OnDragEvent += onDragEndListener;
        }

        public float HeightFraction
        {
            get
            {
                if (_maximumHeight == _minimumHeight) { return 1.0f; }

                float numerator = (_maximumHeight - _rectTransform.localPosition.y);
                float denominator = (_maximumHeight - _minimumHeight);

                return numerator / denominator;
            } 
        }
    }
}
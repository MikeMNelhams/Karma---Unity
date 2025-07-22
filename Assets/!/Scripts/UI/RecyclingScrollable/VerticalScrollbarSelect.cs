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

        float _scrollbarBackgroundHeight;
        float _scrollbarHeight;

        float _minimumHeight;
        float _maximumHeight;

        public delegate void OnDragListener();
        event OnDragListener OnDragEvent;

        public void SetScrollbarHeight(int numberOfViewHolders, float elementHeight, int activeDisplayCount)
        {
            if (numberOfViewHolders == 0)
            {
                throw new DivideByZeroException("The number of elements in the vertical scrollable must not be zero!");
            }

            _scrollbarBackgroundHeight = _scrollbarBackground.rect.height;

            ScaleScrollbarHeight(numberOfViewHolders, elementHeight, activeDisplayCount);

            _rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, _scrollbarHeight);
            SetScrollbarHeightLimits();
            _rectTransform.localPosition = new Vector3(_rectTransform.localPosition.x, _maximumHeight);
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (!eventData.dragging) { return; }

            float yFactor = 1.0f / _parentCanvas.scaleFactor;
            float halfScrollbarHeight = _scrollbarBackgroundHeight / 2.0f;

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
                if (_scrollbarHeight == _scrollbarBackgroundHeight) { return 1.0f; }

                float numerator = (_maximumHeight - _rectTransform.localPosition.y);
                float denominator = (_maximumHeight - _minimumHeight);
                return numerator / denominator;
            } 
        }

        void SetScrollbarHeightLimits()
        {
            float halfScrollbarBackgroundHeight = _scrollbarBackgroundHeight / 2.0f;
            float halfScrollSelectHeight = _scrollbarHeight / 2.0f;

            _maximumHeight = halfScrollbarBackgroundHeight - halfScrollSelectHeight;
            _minimumHeight = -halfScrollbarBackgroundHeight + halfScrollSelectHeight;
        }

        void ScaleScrollbarHeight(int numberOfViewHolders, float elementHeight, int activeDisplayCount)
        {
            if (activeDisplayCount >= numberOfViewHolders)
            {
                _scrollbarHeight = _scrollbarBackgroundHeight;
            }
            else
            {
                _scrollbarHeight = (_scrollbarBackgroundHeight * _scrollbarBackgroundHeight) / (numberOfViewHolders * elementHeight + _scrollbarBackgroundHeight);
            }
        }
    }
}
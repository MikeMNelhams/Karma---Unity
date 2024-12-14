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

        float _scrollSelectHeight;

        float _scrollbarHeight;
        float _minimumHeight;
        float _maximumHeight;

        void Awake()
        {
            _scrollSelectHeight = _rectTransform.rect.height;
            float halfScrollSelectHeight = _scrollSelectHeight / 2.0f;

            _scrollbarHeight = _scrollbarBackground.rect.height;
            _maximumHeight = _scrollbarHeight / 2.0f;
            _minimumHeight =  - _maximumHeight;

            _maximumHeight -= halfScrollSelectHeight;
            _minimumHeight += halfScrollSelectHeight;
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (!eventData.dragging) { return; }
            //float yFactor = ((_parentCanvas.transform as RectTransform).sizeDelta.y) / Screen.height;
            //float yFactor =  1.0f / ((_parentCanvas.transform as RectTransform).sizeDelta.y));
            float yFactor = 1.0f / _parentCanvas.scaleFactor;

            float yNew = Mathf.Clamp(_rectTransform.anchoredPosition.y + eventData.delta.y * yFactor, _minimumHeight, _maximumHeight);
            _rectTransform.anchoredPosition = new Vector2(0, yNew);
            print("Height fraction: " + HeightFraction);
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            print("OnBeginDrag");
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            print("OnEndDrag");
        }

        float HeightFraction 
        { 
            get
            {
                return (_rectTransform.anchoredPosition.y - _minimumHeight) / (_maximumHeight - _minimumHeight);
            } 
        }
    }
}
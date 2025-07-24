using UnityEngine;

namespace CustomUI.RecyclingScrollable
{
    public abstract class ScrollElement : MonoBehaviour
    {
        public delegate void OnClickListener();
        public float Height { get => RectTransform.rect.height; }
        public float Width { get => RectTransform.rect.width; }
        protected abstract RectTransform RectTransform { get; }

        public virtual void SetSize(float width, float height)
        {
            SetWidth(width);
            RectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
        }

        public virtual void SetWidth(float width)
        {
            RectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
        }

        public virtual void SetLocalXPosition(float x)
        {
            RectTransform.localPosition = new Vector3(x, RectTransform.localPosition.y);
        }

        public virtual void SetLocalYPosition(float y)
        {
            RectTransform.localPosition = new Vector3(RectTransform.localPosition.x, y);
        }

        public void SetActive(bool isActive)
        {
            gameObject.SetActive(isActive);
        }
    }
}
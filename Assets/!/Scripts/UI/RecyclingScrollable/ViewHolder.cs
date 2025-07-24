using UnityEngine;

namespace CustomUI.RecyclingScrollable
{
    public abstract class ViewHolder
    {
        [SerializeField] ScrollElement _scrollElement;

        protected ScrollElement ScrollElement { get; }

        public ViewHolder(ScrollElement element)
        {
            _scrollElement = element;
        }

        public void SetActive(bool isActive)
        {
            _scrollElement.SetActive(isActive);
        }

        public void SetWidth(float width)
        {
            _scrollElement.SetWidth(width);
        }

        public void SetYPosition(float yPosition)
        {
            _scrollElement.SetLocalYPosition(yPosition);
        }
    }
}
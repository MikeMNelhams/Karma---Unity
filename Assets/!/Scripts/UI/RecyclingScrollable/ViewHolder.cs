using UnityEngine;

namespace CustomUI.RecyclingScrollable
{
    [System.Serializable]
    public class ViewHolder
    {
        [SerializeField] ScrollElement _scrollElement;

        public ViewHolder(ScrollElement element)
        {
            _scrollElement = element;
        }

        public void SetText(string text)
        {
            _scrollElement.SetText(text);
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

        public void RegisterOnClickListener(ScrollElement.OnClickListener listener)
        {
            _scrollElement.RegisterOnClickListener(listener);
        }
    }
}
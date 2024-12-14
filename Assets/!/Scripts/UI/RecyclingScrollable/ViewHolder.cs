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
    }
}
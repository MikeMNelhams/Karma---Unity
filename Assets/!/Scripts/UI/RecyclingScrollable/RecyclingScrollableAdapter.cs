using UnityEngine;

namespace CustomUI.RecyclingScrollable
{
    public abstract class RecyclingScrollableAdapter : MonoBehaviour
    {
        public abstract ViewHolder OnCreateViewHolder(RectTransform parentRectTransform, GameObject viewHolderPrefab);

        public abstract void OnBindViewHolder(ViewHolder holder, int position);

        public abstract int ItemCount { get; }

        public abstract int SelectedItemIndex { get; set; }
    }
}
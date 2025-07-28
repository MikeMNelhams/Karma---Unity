using UnityEngine;

namespace CustomUI.RecyclingScrollable
{
    public abstract class RecyclingScrollableAdapter : MonoBehaviour
    {
        public abstract ViewHolder CreateViewHolder(RectTransform parentRectTransform, GameObject viewHolderPrefab);

        public abstract void BindViewHolder(ViewHolder holder, int position);

        public virtual void UnbindViewHolder(ViewHolder holder) { return; }

        public abstract int ItemCount { get; }

        public abstract int SelectedItemIndex { get; set; }
    }
}
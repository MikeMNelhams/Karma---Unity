using UnityEngine;

namespace CustomUI.RecyclingScrollable
{
    public abstract class RecyclingScrollableAdapter
    {
        public abstract void OnCreateViewHolder(RecyclingScrollable parent);

        public abstract void OnBindViewHolder(ViewHolder holder, int position);

        public abstract int ItemCount { get; }
    }

}
using NUnit.Framework;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using DataStructures;
using CustomUI.Scrollbar;

namespace CustomUI.RecyclingScrollable
{
    [System.Serializable]
    public class ViewHolderPool
    {
        public ViewHolderPool() { }

        [SerializeField] Deque<ViewHolder> _activeViewHolders;
        [SerializeField] Deque<ViewHolder> _scrapTopViewHolders;
        [SerializeField] Deque<ViewHolder> _scrapBottomViewHolders;
        [SerializeField] List<ViewHolder> _dirtyViewHolders;

        public void InstantiateDirtyViewHolders(GameObject scrollElementPrefab, RectTransform parent, 
            RecyclingScrollableAdapter scrollableAdapter, int activeDisplayCount, int scrapCountPerSide = 2)
        {
            _activeViewHolders = new Deque<ViewHolder>();
            _scrapTopViewHolders = new Deque<ViewHolder>();
            _scrapBottomViewHolders = new Deque<ViewHolder>();
            _dirtyViewHolders = new List<ViewHolder>();

            int totalViewHoldersNeeded = Mathf.Min(scrapCountPerSide * 2 + activeDisplayCount, scrollableAdapter.ItemCount);

            for (int i = 0; i < totalViewHoldersNeeded; i++)
            {
                _dirtyViewHolders.Add(scrollableAdapter.CreateViewHolder(parent, scrollElementPrefab));
            }
        }

        public ViewHolder GetDirtyViewHolder()
        {
            ViewHolder viewHolder = _dirtyViewHolders[^1];
            _dirtyViewHolders.RemoveAt(_dirtyViewHolders.Count - 1);
            return viewHolder;
        }

        public void ScrapAllActiveViewHolders(RecyclingScrollableAdapter _adapter)
        {
            int n = ActiveViewHolders.Count;
            for (int i = 0; i < n; i++)
            {
                ScrapTopActiveViewHolder(_adapter);
            }
        }

        public void ScrapTopActiveViewHolder(RecyclingScrollableAdapter _adapter)
        {
            // TODO Could include async for more efficiency. (Same for the other scrap func)
            //      When scrapping starts, move to scrap deque.
            //      Start a coroutine to make it dirty (unbind the viewholder) and when that's done add to the dirty list
            //      When GetDirtyViewHolder() is called, if there are none ready, it should wait. 
            ViewHolder topViewHolder = ActiveViewHolders.PopLeft();
            _scrapTopViewHolders.AddRight(topViewHolder);
            _adapter.UnbindViewHolder(topViewHolder);
            _dirtyViewHolders.Add(topViewHolder);
        }

        public void ScrapBottomActiveViewHolder(RecyclingScrollableAdapter _adapter)
        {
            ViewHolder bottomViewHolder = ActiveViewHolders.PopRight();
            _scrapBottomViewHolders.AddLeft(bottomViewHolder);
            _adapter.UnbindViewHolder(bottomViewHolder);
            _dirtyViewHolders.Add(bottomViewHolder);
        }

        public Deque<ViewHolder> ActiveViewHolders
        {
            get
            {
                return _activeViewHolders;
            }
        }
    }
}

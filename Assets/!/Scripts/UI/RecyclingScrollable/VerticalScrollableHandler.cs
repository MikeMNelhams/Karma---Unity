using NUnit.Framework;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using DataStructures;
using CustomUI.Scrollbar;

namespace CustomUI.RecyclingScrollable
{
    [RequireComponent(typeof(RectTransform))]
    public class VerticalScrollableHandler : MonoBehaviour
    {
        [SerializeField] RectTransform _rectTransform;

        [SerializeField, HideInInspector] Deque<ViewHolder> _activeViewHolders;
        [SerializeField, HideInInspector] Deque<ViewHolder> _scrapTopViewHolders;
        [SerializeField, HideInInspector] Deque<ViewHolder> _scrapBottomViewHolders;
        [SerializeField, HideInInspector] List<ViewHolder> _dirtyViewHolders;

        int _lowIndex = -1;
        int _highIndex = -1;

        public int LowIndex { get => _lowIndex; set { _lowIndex = value; } }
        public int HighIndex { get => _highIndex; set { _highIndex = value; } }
        
        public int DisplayCount { get { return HighIndex - LowIndex; } }

        public float Height { get => _rectTransform.rect.height; }

        public void InstantiateDirtyViewHolders(GameObject scrollElementPrefab, RecyclingScrollableAdapter scrollableAdapter, int scrapCountPerSide = 2)
        {
            RectTransform elementPrefabRectTransform = scrollElementPrefab.GetComponent<RectTransform>();
            float elementPrefabHeight = elementPrefabRectTransform.rect.height;

            _activeViewHolders = new Deque<ViewHolder>();
            _scrapTopViewHolders = new Deque<ViewHolder>();
            _scrapBottomViewHolders = new Deque<ViewHolder>();

            _dirtyViewHolders = new List<ViewHolder>();

            int totalViewHoldersNeeded = Mathf.Min(scrapCountPerSide * 2 + ActiveDisplayCount(elementPrefabHeight, scrollableAdapter.ItemCount), scrollableAdapter.ItemCount);

            for (int i = 0; i < totalViewHoldersNeeded; i++)
            {
                _dirtyViewHolders.Add(scrollableAdapter.OnCreateViewHolder(_rectTransform, scrollElementPrefab));
            }
        }

        public int ActiveDisplayCount(float elementHeight, int numberOfElements)
        {
            return Mathf.Min(Mathf.CeilToInt(_rectTransform.rect.height / elementHeight) + 1, numberOfElements);
        }

        public ViewHolder GetDirtyViewHolder()
        {
            ViewHolder viewHolder = _dirtyViewHolders[^1];
            _dirtyViewHolders.RemoveAt(_dirtyViewHolders.Count - 1);
            return viewHolder;
        }

        public void ScrapTopActiveViewHolder()
        {
            // TODO This can actually be async, but it would require sync locks.
            ViewHolder topViewHolder = ActiveViewHolders.PopLeft();
            _scrapTopViewHolders.AddRight(topViewHolder);
            topViewHolder.SetActive(false);
            _dirtyViewHolders.Add(topViewHolder);
        }

        public void ScrapBottomActiveViewHolder()
        {
            // TODO This can actually be async, but it would require sync locks.
            ViewHolder bottomViewHolder = ActiveViewHolders.PopRight();
            _scrapBottomViewHolders.AddLeft(bottomViewHolder);
            bottomViewHolder.SetActive(false);
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

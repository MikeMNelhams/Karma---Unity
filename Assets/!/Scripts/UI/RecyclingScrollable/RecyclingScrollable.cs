using CustomUI.Scrollbar;
using UnityEngine;

namespace CustomUI.RecyclingScrollable
{
    public class RecyclingScrollable : MonoBehaviour
    {
        [SerializeField] GameObject _scrollElementPrefab;

        [SerializeField] RectTransform _viewHolderParent;
        [SerializeField] RecyclingScrollableAdapter _adapter;
        [SerializeField] VerticalScrollbar _verticalScrollbar;
        [SerializeField] ViewHolderPool _viewHolderPool;

        int _previousLowIndex = -1;
        int _lowIndex = -1;
        int _highIndex = -1;
        float _elementHeight;

        void Awake()
        {
            RectTransform elementPrefabRectTransform = _scrollElementPrefab.GetComponent<RectTransform>();
            _elementHeight = elementPrefabRectTransform.rect.height;

            int activeDisplayCount = ActiveDisplayCount(_elementHeight, _adapter.ItemCount);

            _lowIndex = 0;
            _highIndex = _lowIndex + activeDisplayCount;

            _viewHolderPool = new ();
            _viewHolderPool.InstantiateDirtyViewHolders(_scrollElementPrefab, _viewHolderParent, _adapter, ActiveDisplayCount(_elementHeight, _adapter.ItemCount));
        }

        void Start()
        {
            ResetScrollable();
        }

        int ActiveDisplayCount(float elementHeight, int numberOfElements)
        {
            return Mathf.Min(Mathf.CeilToInt(_viewHolderParent.rect.height / elementHeight) + 1, numberOfElements);
        }

        float ParentHeight
        {
            get { return _viewHolderParent.rect.height; }
        }

        public void ResetScrollable()
        {
            ScrapAllActiveViewHolders();

            int activeDisplayCount = ActiveDisplayCount(_elementHeight, _adapter.ItemCount);
            _verticalScrollbar.SetScrollbarHeight(_adapter.ItemCount, _elementHeight, activeDisplayCount);
            InitializeViewHolders();
            _verticalScrollbar.RegisterOnDragListener(UpdateViewHolders);
        }

        void ScrapAllActiveViewHolders()
        {
            _viewHolderPool.ScrapAllActiveViewHolders(_adapter);
        }

        void InitializeViewHolders()
        {
            int activeDisplayCount = ActiveDisplayCount(_elementHeight, _adapter.ItemCount);

            if (activeDisplayCount >= _adapter.ItemCount)
            {
                _lowIndex = 0;
                _highIndex = _adapter.ItemCount - 1;

                InstantiateStartingViewHolders();
                UpdateViewHolderActivePositionsNotFillsScrollableHandler();
            }
            else
            {
                _lowIndex = LowIndex(activeDisplayCount, _verticalScrollbar.ScrollFraction);
                _highIndex = _lowIndex + activeDisplayCount - 1;

                InstantiateStartingViewHolders();
                UpdateViewHolderActivePositions();
            }

            _adapter.SelectedItemIndex = _adapter.SelectedItemIndex;
            _previousLowIndex = _lowIndex;
        }

        void UpdateViewHolders()
        {
            int activeDisplayCount = ActiveDisplayCount(_elementHeight, _adapter.ItemCount);
            if (_adapter.ItemCount < activeDisplayCount) { return; }

            _lowIndex = LowIndex(activeDisplayCount, _verticalScrollbar.ScrollFraction);
            _highIndex = HighIndex(activeDisplayCount);

            if (_lowIndex == _previousLowIndex)
            {
                UpdateViewHolderActivePositions();
                return;
            }

            if (_lowIndex > _previousLowIndex)
            {
                ScrollDownScrapFirstmost();
                UpdateViewHolderActivePositions();
                return;
            }

            if (_lowIndex < _previousLowIndex)
            {
                ScrollUpScrapLastmost();
                UpdateViewHolderActivePositions();
                return;
            }

            throw new System.Exception("UpdateViewHolder positions error.");
        }

        int LowIndex(int activeDisplayCount, float heightFraction)
        {
            return Mathf.Max(Mathf.FloorToInt(heightFraction * (_adapter.ItemCount - activeDisplayCount + 1)), 0);
        }

        int HighIndex(int activeDisplayCount)
        {
            return Mathf.Min(_lowIndex + activeDisplayCount - 1, _adapter.ItemCount - 1);
        }

        void UpdateViewHolderActivePositions()
        {
            float windowOffset = _verticalScrollbar.ScrollFraction * (_adapter.ItemCount * _elementHeight - ParentHeight);
            int activeIndex = 0;
            for (int i = _lowIndex; i <= _highIndex; i++) 
            {
                ViewHolder viewHolder = _viewHolderPool.ActiveViewHolders[activeIndex];
                viewHolder.SetYPosition(windowOffset + 0.5f * ParentHeight - _elementHeight * (i + 0.5f));
                activeIndex++;
            }
        }

        void ScrollDownScrapFirstmost()
        {
            for (int i = 0; i < _lowIndex - _previousLowIndex; i++)
            {
                _previousLowIndex++;
                _viewHolderPool.ScrapTopActiveViewHolder(_adapter);

                ViewHolder viewHolder = _viewHolderPool.GetDirtyViewHolder();
                _adapter.BindViewHolder(viewHolder, _highIndex - i);
                _viewHolderPool.ActiveViewHolders.AddRight(viewHolder);
            }
        }

        void ScrollUpScrapLastmost()
        {
            for (int i = 0; i < _previousLowIndex - _lowIndex; i++)
            {
                _previousLowIndex--;
                _viewHolderPool.ScrapBottomActiveViewHolder(_adapter);

                ViewHolder viewHolder = _viewHolderPool.GetDirtyViewHolder();
                _adapter.BindViewHolder(viewHolder, _lowIndex + i);
                _viewHolderPool.ActiveViewHolders.AddLeft(viewHolder);
            }
        }

        void InstantiateStartingViewHolders()
        {
            for (int i = _lowIndex; i <= _highIndex; i++)
            {
                ViewHolder viewHolder = _viewHolderPool.GetDirtyViewHolder();
                _adapter.BindViewHolder(viewHolder, i);
                _viewHolderPool.ActiveViewHolders.AddRight(viewHolder);
            }
        }

        void UpdateViewHolderActivePositionsNotFillsScrollableHandler()
        {
            if (_adapter.ItemCount == 0) { return; }

            float emptySpace = ParentHeight - _adapter.ItemCount * _elementHeight;
            float windowOffset = (0.5f - _verticalScrollbar.ScrollFraction) * emptySpace;

            int activeIndex = 0;
            for (int i = _lowIndex; i <= _highIndex; i++)
            {
                ViewHolder viewHolder = _viewHolderPool.ActiveViewHolders[activeIndex];
                viewHolder.SetYPosition(0.5f * ParentHeight + windowOffset - _elementHeight * (i + 0.5f));
                activeIndex++;
            }
        }
    }
}
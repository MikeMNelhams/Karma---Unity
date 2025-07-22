using CustomUI.Scrollbar;
using UnityEngine;
using UnityEngine.UI;

namespace CustomUI.RecyclingScrollable
{
    public class RecyclingScrollable : MonoBehaviour
    {
        [SerializeField] GameObject _scrollElementPrefab;

        [SerializeField] VerticalScrollbarSelect _scrollbarSelect;

        int _previousLowIndex = -1;
        int _lowIndex = -1;
        int _highIndex = -1;
        float _elementHeight;

        [SerializeField] RecyclingScrollableAdapter _adapter;
        [SerializeField] VerticalScrollableHandler _scrollableHandler;

        void Awake()
        {
            RectTransform elementPrefabRectTransform = _scrollElementPrefab.GetComponent<RectTransform>();
            _elementHeight = elementPrefabRectTransform.rect.height;

            int activeDisplayCount = _scrollableHandler.ActiveDisplayCount(_elementHeight, _adapter.ItemCount);

            _lowIndex = 0;
            _highIndex = _lowIndex + activeDisplayCount;

            _scrollableHandler.InstantiateDirtyViewHolders(_scrollElementPrefab, _adapter);
        }

        void Start()
        {
            int activeDisplayCount = _scrollableHandler.ActiveDisplayCount(_elementHeight, _adapter.ItemCount);
            _scrollbarSelect.SetScrollbarHeight(_adapter.ItemCount, _elementHeight, activeDisplayCount);
            InitializeViewHolders();
            _scrollbarSelect.RegisterOnDragListener(UpdateViewHolders);
        }

        void InitializeViewHolders()
        {
            int activeDisplayCount = _scrollableHandler.ActiveDisplayCount(_elementHeight, _adapter.ItemCount);

            if (activeDisplayCount >= _adapter.ItemCount)
            {
                _lowIndex = 0;
                _highIndex = _adapter.ItemCount - 1;

                InstantiateStartingViewHolders();
                UpdateViewHolderActivePositionsNotFillsScrollableHandler();
            }
            else
            {
                _lowIndex = LowIndex(activeDisplayCount, _scrollbarSelect.HeightFraction);
                _highIndex = _lowIndex + activeDisplayCount - 1;

                InstantiateStartingViewHolders();
                UpdateViewHolderActivePositions();
            }

            _adapter.SelectedItemIndex = _adapter.SelectedItemIndex;
            _previousLowIndex = _lowIndex;
        }

        void UpdateViewHolders()
        {
            // If low = previous_low, update all Ypositions of activeViewHolders NO REBINDS
            // If low = previous_low + 1, scrap lowest index (top) viewHolder. GetDirtyViewHolder(). bind to highIndex. update Ypositions of activeViewHolders
            // If low = previous_low - 1, scrap highest index (bottom) viewHolder. GetDirtyViewHolder(). bind to lowIndex. update Ypositions of activeViewHolders

            int activeDisplayCount = _scrollableHandler.ActiveDisplayCount(_elementHeight, _adapter.ItemCount);
            if (_adapter.ItemCount < activeDisplayCount) { return; }

            float heightFraction = _scrollbarSelect.HeightFraction;
            _lowIndex = LowIndex(activeDisplayCount, heightFraction);
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
            float windowOffset = _scrollbarSelect.HeightFraction * (_adapter.ItemCount * _elementHeight - _scrollableHandler.Height);
            int activeIndex = 0;
            for (int i = _lowIndex; i <= _highIndex; i++) 
            {
                ViewHolder viewHolder = _scrollableHandler.ActiveViewHolders[activeIndex];
                viewHolder.SetYPosition(windowOffset + 0.5f * _scrollableHandler.Height - _elementHeight * (i + 0.5f));
                activeIndex++;
            }
        }

        void ScrollDownScrapFirstmost()
        {
            for (int i = 0; i < _lowIndex - _previousLowIndex; i++)
            {
                _previousLowIndex++;
                _scrollableHandler.ScrapTopActiveViewHolder();

                ViewHolder viewHolder = _scrollableHandler.GetDirtyViewHolder();
                _adapter.OnBindViewHolder(viewHolder, _highIndex - i);
                _scrollableHandler.ActiveViewHolders.AddRight(viewHolder);
            }
        }

        void ScrollUpScrapLastmost()
        {
            for (int i = 0; i < _previousLowIndex - _lowIndex; i++)
            {
                _previousLowIndex--;
                _scrollableHandler.ScrapBottomActiveViewHolder();

                ViewHolder viewHolder = _scrollableHandler.GetDirtyViewHolder();
                _adapter.OnBindViewHolder(viewHolder, _lowIndex + i);
                _scrollableHandler.ActiveViewHolders.AddLeft(viewHolder);
            }
        }

        void InstantiateStartingViewHolders()
        {
            for (int i = _lowIndex; i <= _highIndex; i++)
            {
                ViewHolder viewHolder = _scrollableHandler.GetDirtyViewHolder();
                _adapter.OnBindViewHolder(viewHolder, i);
                _scrollableHandler.ActiveViewHolders.AddRight(viewHolder);
            }
        }

        void UpdateViewHolderActivePositionsNotFillsScrollableHandler()
        {
            if (_adapter.ItemCount == 0)
            {
                return;
            }

            float emptySpace = _scrollableHandler.Height - _adapter.ItemCount * _elementHeight;
            float windowOffset = (0.5f - _scrollbarSelect.HeightFraction) * emptySpace;

            int activeIndex = 0;
            for (int i = _lowIndex; i <= _highIndex; i++)
            {
                ViewHolder viewHolder = _scrollableHandler.ActiveViewHolders[activeIndex];
                viewHolder.SetYPosition(0.5f * _scrollableHandler.Height + windowOffset - _elementHeight * (i + 0.5f));
                activeIndex++;
            }
        }
    }
}
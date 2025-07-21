using CustomUI.Scrollbar;
using UnityEngine;
using UnityEngine.UI;

namespace CustomUI.RecyclingScrollable
{
    public class RecyclingScrollable : MonoBehaviour
    {
        [SerializeField] GameObject _scrollElementPrefab;

        [SerializeField] VerticalScrollbarSelect _scrollbarSelect;

        // Test text. This will be replaced with the correct board preset names.

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

            if (activeDisplayCount == _adapter.ItemCount) 
            {
                _lowIndex = 0;
                _highIndex = _adapter.ItemCount - 1; 
            }
            else
            {
                _lowIndex = LowIndex(activeDisplayCount, _scrollbarSelect.HeightFraction);
                _highIndex = _lowIndex + activeDisplayCount - 1;
            }

            for (int i = _lowIndex; i <= _highIndex; i++)
            {
                ViewHolder viewHolder = _scrollableHandler.GetDirtyViewHolder();
                _adapter.OnBindViewHolder(viewHolder, i);
                _scrollableHandler.ActiveViewHolders.AddRight(viewHolder);
            }

            _adapter.SelectedItemIndex = _adapter.SelectedItemIndex;
            UpdateViewHolderActivePositions();

            _previousLowIndex = _lowIndex;
        }

        void UpdateViewHolders()
        {
            //   If low = previous_low then update all the Ypositions of the activeViewHolders NO REBINDS
            //   If low = previous_low + 1 then scrap the lowest index (top) viewHolder. GetDirtyViewHolder() and bind it to highIndex. then update the Ypositions of activeViewHolders
            //   If low = previous_low - 1 then scrap the highest index (bottom) viewHolder. GetDirtyViewHolder() and bind it to lowIndex. then update the Ypositions of activeViewHolders

            int activeDisplayCount = _scrollableHandler.ActiveDisplayCount(_elementHeight, _adapter.ItemCount);
            if (_adapter.ItemCount <= activeDisplayCount) { return; }

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
            int activeIndex = 0;
            for (int i = _lowIndex; i <= _highIndex; i++) 
            {
                ViewHolder viewHolder = _scrollableHandler.ActiveViewHolders[activeIndex];
                float windowOffset = _scrollbarSelect.HeightFraction * (_adapter.ItemCount * _elementHeight - _scrollableHandler.Height);
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
    }
}
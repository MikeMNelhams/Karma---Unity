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
            _scrollbarSelect.SetScrollbarHeight(_adapter.ItemCount, _elementHeight, _scrollableHandler.ActiveDisplayCount(_elementHeight, _adapter.ItemCount));

            InitializeViewHolders();

            _scrollbarSelect.RegisterOnDragListener(UpdateViewHolders);
        }

        void InitializeViewHolders()
        {
            float heightFraction = _scrollbarSelect.HeightFraction;

            int activeDisplayCount = _scrollableHandler.ActiveDisplayCount(_elementHeight, _adapter.ItemCount);

            int lowIndexPossiblyOverflow = Mathf.FloorToInt(heightFraction * (float)_adapter.ItemCount);
            int maximumLowIndex = _adapter.ItemCount - activeDisplayCount;

            _lowIndex = Mathf.Max(Mathf.Min(lowIndexPossiblyOverflow, maximumLowIndex) - 1, 0);
            _highIndex = _lowIndex + activeDisplayCount;

            for (int i = _lowIndex; i < _highIndex; i++)
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
            // Instead this should be:
            //      If low = previous_low then update all the Ypositions of the activeViewHolders NO REBINDS
            //      If low = previous_low + 1 then scrap the lowest index (top) viewHolder. GetDirtyViewHolder() and bind it to highIndex. then update all the Ypositions of the activeViewHolders
            //      If low = previous_low - 1 then scrap the highest index (bottom) viewHolder. GetDirtyViewHolder() and bind it to lowIndex. then update all the Ypositions of the activeViewHolders

            float heightFraction = _scrollbarSelect.HeightFraction;

            int activeDisplayCount = _scrollableHandler.ActiveDisplayCount(_elementHeight, _adapter.ItemCount);
            int lowIndexPossiblyOverflow = Mathf.FloorToInt(heightFraction * (float)_adapter.ItemCount);

            int maximumLowIndex = _adapter.ItemCount - activeDisplayCount;
            _lowIndex = Mathf.Max(Mathf.Min(lowIndexPossiblyOverflow, maximumLowIndex) - 1, 0);
            _highIndex = _lowIndex + activeDisplayCount;

            if (_lowIndex == _previousLowIndex)
            {
                UpdateViewHolderActivePositions();
                return;
            }
            if (_lowIndex > _previousLowIndex)
            {
                for (int i = 0; i < _lowIndex - _previousLowIndex; i++)
                {
                    _previousLowIndex++;
                    _scrollableHandler.ScrapTopActiveViewHolder();

                    ViewHolder viewHolder = _scrollableHandler.GetDirtyViewHolder();
                    _adapter.OnBindViewHolder(viewHolder, _highIndex - i);
                    _scrollableHandler.ActiveViewHolders.AddRight(viewHolder);
                }

                UpdateViewHolderActivePositions();
                return;
            }

            if (_lowIndex < _previousLowIndex)
            {
                for (int i = 0; i < _previousLowIndex - _lowIndex; i++)
                {
                    _previousLowIndex--;
                    _scrollableHandler.ScrapBottomActiveViewHolder();

                    ViewHolder viewHolder = _scrollableHandler.GetDirtyViewHolder();
                    _adapter.OnBindViewHolder(viewHolder, _lowIndex + i);
                    _scrollableHandler.ActiveViewHolders.AddLeft(viewHolder);
                }

                UpdateViewHolderActivePositions();
                return;
            }

            throw new System.Exception("UpdateViewHolder positions error.");
        }

        void UpdateViewHolderActivePositions(float topPadding = 5.0f)
        {
            float heightFraction = _scrollbarSelect.HeightFraction;

            int activeViewHoldersCount = _scrollableHandler.ActiveViewHolders.Count;
            int unshownViewHoldersCount = _adapter.ItemCount - activeViewHoldersCount;

            float windowStartFraction = heightFraction * (unshownViewHoldersCount);

            for (int i = 0; i < activeViewHoldersCount; i++)
            {
                ViewHolder viewHolder = _scrollableHandler.ActiveViewHolders[i];
                float y = -1 * ((_lowIndex + i - 1.5f) - windowStartFraction) * _elementHeight - topPadding;
                viewHolder.SetYPosition(y);
            }
        }
    }
}
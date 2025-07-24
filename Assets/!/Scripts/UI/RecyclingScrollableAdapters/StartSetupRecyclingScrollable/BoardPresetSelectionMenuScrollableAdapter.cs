using UnityEngine;
using System;
using KarmaPlayerMode;
using System.Collections.Generic;

namespace CustomUI.RecyclingScrollable.SingleplayerSetup
{
    public class BoardPresetSelectionMenuScrollableAdapter : RecyclingScrollableAdapter
    {
        [SerializeField] PresetDisplayInfo[] _presetData;
        [SerializeField] int _selectedIndex;
        [SerializeField] SelectedBoardPresetDisplay _selectedBoardPresetDisplay;

        Dictionary<int, Action> _playerModeSettingMap;

        private void Awake()
        {
            _playerModeSettingMap = new()
        {
            {0, SelectSingleplayerSolo},
            {1, SelectSingleplayerCo_op}
        };
        }

        void SelectSingleplayerSolo()
        {
            KarmaGameManager.Instance.SetPlayerMode(PlayerMode.Singleplayer);
            KarmaGameManager.Instance.SetPlayerSubMode((int)SinglePlayerMode.Solo);
            KarmaGameManager.Instance.SetSelectedBoardPreset(23);
        }

        void SelectSingleplayerCo_op()
        {
            KarmaGameManager.Instance.SetPlayerMode(PlayerMode.Singleplayer);
            KarmaGameManager.Instance.SetPlayerSubMode((int)SinglePlayerMode.Many);
            KarmaGameManager.Instance.SetSelectedBoardPreset(0);
        }

        public override int ItemCount => _presetData.Length;

        public override int SelectedItemIndex
        {
            get => _selectedIndex;
            set
            {
                _selectedIndex = value;
                _selectedBoardPresetDisplay.SetDisplayedText(_presetData[_selectedIndex]);
            }
        }

        void OnViewHolderClick(int position)
        {
            SelectedItemIndex = position;
            KarmaGameManager.Instance.SetIsUsingBoardPresets(true);
            if (!_playerModeSettingMap.ContainsKey(position)) { throw new KeyNotFoundException(position.ToString()); }
            _playerModeSettingMap[position]();
        }

        public override void OnBindViewHolder(ViewHolder holder, int position)
        {
            ViewHolderSingleplayerSetup viewHolderSingleplayerSetup = (ViewHolderSingleplayerSetup)holder;
            viewHolderSingleplayerSetup.SetActive(true);
            viewHolderSingleplayerSetup.RegisterOnClickListener(() => OnViewHolderClick(position));
            viewHolderSingleplayerSetup.SetText(_presetData[position].TitleText);
        }

        public override ViewHolder OnCreateViewHolder(RectTransform parentRectTransform, GameObject viewHolderPrefab)
        {
            if (viewHolderPrefab == null) { throw new MissingReferenceException(); }

            GameObject viewHolderGameObject = GameObject.Instantiate(viewHolderPrefab, parentRectTransform.gameObject.transform);

            if (!viewHolderGameObject.TryGetComponent<SingleplayerSetupScrollElement>(out var scrollElement)) { throw new MissingComponentException(); }
            viewHolderGameObject.SetActive(false);

            scrollElement.SetWidth(parentRectTransform.rect.width);
            scrollElement.SetLocalXPosition(0);

            return new ViewHolderSingleplayerSetup(scrollElement);
        }
    }
}
using UnityEngine;
using KarmaPlayerMode;
using CustomUI.RecyclingScrollable.MultiplayerLobby;

namespace CustomUI.RecyclingScrollable
{
    public class MultiplayerLobbyPlayersScrollableAdapter : RecyclingScrollableAdapter
    {
        int _playersInLobbyCount = 4;
        bool _selfPlayerSet = false;

        void SelectMultiplayer()
        {
            KarmaGameManager.Instance.SetPlayerMode(PlayerMode.Multiplayer);
            KarmaGameManager.Instance.SetSelectedBoardPreset(0);
        }

        public override int ItemCount => _playersInLobbyCount;

        public override int SelectedItemIndex { get => 0; set { return; } }

        public override void OnBindViewHolder(ViewHolder holder, int position)
        {
            ViewHolderMultiplayerLobby holderMultiplayerLobby = (ViewHolderMultiplayerLobby)holder;

            if (!_selfPlayerSet && position == 0)
            {
                InitializeSelfPosition(holder as ViewHolderMultiplayerLobby);
                return;
            }

            holderMultiplayerLobby.SetActive(true);
            holderMultiplayerLobby.SetPlayerNameText("Waiting for player...");
        }

        public override ViewHolder OnCreateViewHolder(RectTransform parentRectTransform, GameObject viewHolderPrefab)
        {
            GameObject viewHolderGameObject = GameObject.Instantiate(viewHolderPrefab, parentRectTransform.gameObject.transform);

            if (!viewHolderGameObject.TryGetComponent<MultiplayerLobbyScrollElement>(out var scrollElement)) { throw new MissingComponentException(); }
            viewHolderGameObject.SetActive(false);

            scrollElement.SetWidth(parentRectTransform.rect.width);
            scrollElement.SetLocalXPosition(0);

            return new ViewHolderMultiplayerLobby(scrollElement);
        }

        void InitializeSelfPosition(ViewHolderMultiplayerLobby holder)
        {
            ViewHolderMultiplayerLobby holderMultiplayerLobby = (ViewHolderMultiplayerLobby)holder;
            holderMultiplayerLobby.SetActive(true);
            holderMultiplayerLobby.SetPlayerNameText("INSERT SELF STEAM NAME HERE!");
            _selfPlayerSet = true;
        }
    }
}
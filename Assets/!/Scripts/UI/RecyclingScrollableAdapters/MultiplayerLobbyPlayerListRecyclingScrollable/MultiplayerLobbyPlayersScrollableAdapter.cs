using UnityEngine;
using CustomUI.RecyclingScrollable.MultiplayerLobby;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine.UI;

namespace CustomUI.RecyclingScrollable
{
    public class MultiplayerLobbyPlayersScrollableAdapter : RecyclingScrollableAdapter
    {
        [SerializeField] Button _backButton;

        int _lobbyPlayersCapacity = 10;
        readonly Dictionary<ulong, int> _readyPlayerPositions = new ();
        readonly Dictionary<int, ulong> _readyPositionsToPlayers = new ();

        

        void Awake()
        {
            _backButton.onClick.AddListener(ResetReadiedPlayers);
        }

        public override int ItemCount => _lobbyPlayersCapacity;

        public override int SelectedItemIndex { get => 0; set { return; } }

        public override void BindViewHolder(ViewHolder holder, int position)
        {
            print($"Binding view holder: {position}");
            ViewHolderMultiplayerLobby holderMultiplayerLobby = (ViewHolderMultiplayerLobby)holder;
            
            holderMultiplayerLobby.SetActive(true);

            if (_readyPositionsToPlayers.ContainsKey(position))
            {

            }

            holderMultiplayerLobby.SetPlayerNameText("Waiting for player...");
            holderMultiplayerLobby.SetPlayerNumberText((position + 1).ToString());

            holderMultiplayerLobby.ConfirmButton.onClick.AddListener(delegate 
            {
                OnViewHolderClick(holderMultiplayerLobby, position, NetworkManager.Singleton.LocalClientId); 
            });
        }

        public override void UnbindViewHolder(ViewHolder holder)
        {
            ViewHolderMultiplayerLobby holderMultiplayerLobby = (ViewHolderMultiplayerLobby)holder;
            holderMultiplayerLobby.ConfirmButton.onClick.RemoveAllListeners();
        }

        public override ViewHolder CreateViewHolder(RectTransform parentRectTransform, GameObject viewHolderPrefab)
        {
            GameObject viewHolderGameObject = GameObject.Instantiate(viewHolderPrefab, parentRectTransform.gameObject.transform);

            if (!viewHolderGameObject.TryGetComponent<MultiplayerLobbyScrollElement>(out var scrollElement)) { throw new MissingComponentException(); }
            viewHolderGameObject.SetActive(false);

            scrollElement.SetWidth(parentRectTransform.rect.width);
            scrollElement.SetLocalXPosition(0);

            return new ViewHolderMultiplayerLobby(scrollElement);
        }

        void OnViewHolderClick(ViewHolderMultiplayerLobby holder, int position, ulong clientID)
        {
            if (_readyPlayerPositions.ContainsKey(clientID)) { return; }

            holder.SetPlayerNameText("PLAYER NAME");
            holder.SetPlayerNumberText((position + 1).ToString());

            _readyPlayerPositions[clientID] = position;
            _readyPositionsToPlayers[position] = clientID;
        }

        void ResetReadiedPlayers()
        {
            _readyPlayerPositions.Clear();
            _readyPositionsToPlayers.Clear();
        }
    }
}
using UnityEngine;
using UnityEngine.UI;

namespace CustomUI.RecyclingScrollable.MultiplayerLobby
{
    [System.Serializable]
    public class ViewHolderMultiplayerLobby : ViewHolder
    {
        MultiplayerLobbyScrollElement _element;

        public Button ConfirmButton { get => _element.ConfirmButton; }

        public ViewHolderMultiplayerLobby(MultiplayerLobbyScrollElement element) : base(element)
        {
            _element = element;
        }

        public void SetPlayerNameText(string playerName)
        {
            _element.SetPlayerNameText(playerName);
        }

        public void SetPlayerNumberText(string playerNumber)
        {
            _element.SetPlayerNumberText(playerNumber);
        }
    }
}
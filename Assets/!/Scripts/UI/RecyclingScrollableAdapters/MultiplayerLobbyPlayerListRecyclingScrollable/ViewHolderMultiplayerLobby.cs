using UnityEngine;

namespace CustomUI.RecyclingScrollable.MultiplayerLobby
{
    [System.Serializable]
    public class ViewHolderMultiplayerLobby : ViewHolder
    {

        MultiplayerLobbyScrollElement _element;

        public ViewHolderMultiplayerLobby(MultiplayerLobbyScrollElement element) : base(element)
        {
            _element = element;
        }

        public void SetPlayerNameText(string playerName)
        {
            _element.SetPlayerNameText(playerName);
        }
    }
}
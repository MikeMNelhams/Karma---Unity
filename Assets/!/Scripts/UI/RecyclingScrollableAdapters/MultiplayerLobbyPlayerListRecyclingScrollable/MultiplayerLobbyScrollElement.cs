using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CustomUI.RecyclingScrollable.MultiplayerLobby
{
    public class MultiplayerLobbyScrollElement : ScrollElement
    {
        [SerializeField] RectTransform _rectTransform;
        [SerializeField] TextMeshProUGUI _playerNameText;
        [SerializeField] Button _kickButton;

        protected override RectTransform RectTransform { get => _rectTransform; }

        public void SetPlayerNameText(string text)
        {
            _playerNameText.text = text;
        }

    }
}


using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CustomUI.RecyclingScrollable.MultiplayerLobby
{
    public class MultiplayerLobbyScrollElement : ScrollElement
    {
        [SerializeField] RectTransform _rectTransform;
        [SerializeField] TextMeshProUGUI _playerNameText;
        [SerializeField] TextMeshProUGUI _playerNumberText;
        [SerializeField] Button _confirmButton;

        protected override RectTransform RectTransform { get => _rectTransform; }
        public Button ConfirmButton { get => _confirmButton; }

        public void SetPlayerNameText(string text)
        {
            _playerNameText.text = text;
        }

        public void SetPlayerNumberText(string text)
        {
            _playerNumberText.text = text;
        }
    }
}


using UnityEngine;
using UnityEngine.UI;
using CustomUI.Menu;
using Unity.Netcode;

namespace Pages
{
    [RequireComponent(typeof(CanvasGroup), typeof(RectTransform))]
    public class MultiplayerLobbyPage : Page
    {
        [SerializeField] Button _backButton;

        void Awake()
        {
            _backButton.onClick.AddListener
            (
                () =>
                {
                    if (!NetworkManager.Singleton.IsHost) { return; }
                    NetworkManager.Singleton.Shutdown();
                }
            );
        }
    }
}


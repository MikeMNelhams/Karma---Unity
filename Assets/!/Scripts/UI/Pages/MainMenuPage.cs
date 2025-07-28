using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

namespace CustomUI.Menu.Pages
{
    [RequireComponent(typeof(CanvasGroup), typeof(RectTransform))]
    public class MainMenuPage : Page
    {
        [SerializeField] Button _singleplayerDebugButton;
        [SerializeField] Button _joinMultiplayerButton;
        [SerializeField] Button _hostMultiplayerButton;
        
        void Awake()
        {
#if UNITY_EDITOR
            _singleplayerDebugButton.enabled = true;
#else
        _singleplayerDebugButton.enabled = false;
#endif
            _hostMultiplayerButton.onClick.AddListener(HostMultiplayer);
            _joinMultiplayerButton.onClick.AddListener(JoinMultiplayer);

        }

        void HostMultiplayer()
        {
            NetworkManager.Singleton.StartHost();
        }

        void JoinMultiplayer()
        {
            NetworkManager.Singleton.StartClient();
        }
    }
}
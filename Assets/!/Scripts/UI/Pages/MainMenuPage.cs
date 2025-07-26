using UnityEngine;
using UnityEngine.UI;
using CustomUI.Menu;
using Unity.Netcode;


namespace Pages
{
    [RequireComponent(typeof(CanvasGroup), typeof(RectTransform))]
    public class MainMenuPage : Page
    {
        [SerializeField] Button _singleplayerDebugButton;
        [SerializeField] Button _multiplayerButton;

        void Awake()
        {
#if UNITY_EDITOR
            _singleplayerDebugButton.enabled = true;
#else
        _singleplayerDebugButton.enabled = false;
#endif
            _multiplayerButton.onClick.AddListener
            (
                () =>
                {
                    if (NetworkManager.Singleton.IsHost) { return; }
                    NetworkManager.Singleton.StartHost();
                }
            );
        }
    }
}
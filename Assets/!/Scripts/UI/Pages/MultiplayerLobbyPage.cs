using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;

namespace CustomUI.Menu.Pages
{
    public class MultiplayerLobbyPage : Page
    {
        [Header("Buttons")]
        [SerializeField] Button _startGameButton;
        [SerializeField] Button _backButton;

        private void Awake()
        {
            _startGameButton.onClick.AddListener(TryStartGame);
            _backButton.onClick.AddListener(DisconnectSelf);
        }

        void DisconnectSelf()
        {
            NetworkManager.Singleton.Shutdown();
        }

        void TryStartGame()
        {
            if (!NetworkManager.Singleton.IsServer)
            {
                UnityEngine.Debug.Log("No server is connected. Join one of the player slots.", this);
                return;
            }

            if (!NetworkManager.Singleton.IsHost && !NetworkManager.Singleton.IsServer) 
            {
                UnityEngine.Debug.Log("Only the host may start the game!", this);
                return; 
            }

            int playerCount = NetworkManager.Singleton.ConnectedClientsList.Count;
            if (playerCount <= 1) 
            {
                UnityEngine.Debug.Log("Need 2 or more players to start a multiplayer game...", this);
                return; 
            }

            UnityEngine.Debug.Log($"Starting game with {playerCount} players", this);

            MenuUIManager.Instance.PopPage();
        }
    }
}
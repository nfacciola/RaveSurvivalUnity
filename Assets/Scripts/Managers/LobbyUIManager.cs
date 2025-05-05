using Steamworks;
using TMPro;
using Mirror;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

namespace RaveSurvival
{
    public class LobbyUIManager : NetworkBehaviour
    {
        // Singleton instance of the LobbyUIManager
        public static LobbyUIManager Instance;

        // Parent transform for the player list UI
        public Transform playerListParent;

        // List of TextMeshProUGUI components for displaying player names
        public List<TextMeshProUGUI> playerNameTexts = new List<TextMeshProUGUI>();

        // List of PlayerLobbyHandler components for managing player states
        public List<PlayerLobbyHandler> playerLobbyHandlers = new List<PlayerLobbyHandler>();

        // Button to start the game
        public Button playGameButton;

        /// <summary>
        /// Unity's Awake method, called when the script instance is being loaded.
        /// Sets up the singleton instance of the LobbyUIManager.
        /// </summary>
        void Awake()
        {
            if (Instance == null)
            {
                Instance = this; // Assign this instance as the singleton
            }
            else if (Instance != this)
            {
                Destroy(gameObject); // Destroy duplicate instances
                return;
            }
        }

        /// <summary>
        /// Unity's Start method, called before the first frame update.
        /// Initializes the play game button to be non-interactable.
        /// </summary>
        void Start()
        {
            playGameButton.interactable = false;
        }

        /// <summary>
        /// Updates the player lobby UI with the current list of players in the lobby.
        /// </summary>
        public void UpdatePlayerLobbyUI()
        {
            // Clear existing player name texts and handlers
            playerNameTexts.Clear();
            playerLobbyHandlers.Clear();

            // Get the lobby ID and log it
            var lobby = new CSteamID(SteamLobby.Instance.lobbyID);
            Debug.Log($"Get Lobby ID: {SteamLobby.Instance.lobbyID}");

            // Get the number of members in the lobby
            int memberCount = SteamMatchmaking.GetNumLobbyMembers(lobby);
            Debug.Log($"Updating player names. Lobby member count: {memberCount}");

            // Get the host ID from the lobby data
            CSteamID hostID = new CSteamID(ulong.Parse(SteamMatchmaking.GetLobbyData(lobby, "HostAddress")));
            List<CSteamID> orderedMembers = new List<CSteamID>();

            // If the lobby has no members, retry the update
            if (memberCount == 0)
            {
                Debug.LogWarning("Lobby has no members yet, retrying...");
                StartCoroutine(RetryUpdate());
                return;
            }

            // Add the host to the ordered list first
            orderedMembers.Add(hostID);

            // Add all other members except the host
            for (int i = 0; i < memberCount; i++)
            {
                CSteamID memberID = SteamMatchmaking.GetLobbyMemberByIndex(lobby, i);
                if (memberID != hostID)
                {
                    orderedMembers.Add(memberID);
                }
            }

            // Update the UI for each player in the ordered list
            int j = 0;
            foreach (var member in orderedMembers)
            {
                // Get the UI elements for the player
                TextMeshProUGUI txtMesh = playerListParent.GetChild(j).GetChild(0).GetComponent<TextMeshProUGUI>();
                PlayerLobbyHandler playerLobbyHandler = playerListParent.GetChild(j).GetComponent<PlayerLobbyHandler>();

                // Add the player to the lists
                playerLobbyHandlers.Add(playerLobbyHandler);
                playerNameTexts.Add(txtMesh);

                // Get the player's name and update the UI
                string playerName = SteamFriends.GetFriendPersonaName(member);
                playerNameTexts[j].text = playerName;
                j++;
            }
        }

        /// <summary>
        /// Called when the play button is clicked.
        /// Changes the scene to the gameplay scene if the server is active.
        /// </summary>
        public void OnPlayButtonClicked()
        {
            if (NetworkServer.active)
            {
                LobbyNetworkManager.singleton.ServerChangeScene("GameplayScene");
            }
        }

        /// <summary>
        /// Registers a new player in the lobby and updates the UI.
        /// </summary>
        /// <param name="player">The player to register</param>
        public void RegisterPlayer(PlayerLobbyHandler player)
        {
            // Set the player's parent to the player list UI
            player.transform.SetParent(playerListParent, false);

            // Update the lobby UI
            UpdatePlayerLobbyUI();
        }

        /// <summary>
        /// Checks if all players are ready and updates the play button's interactable state.
        /// </summary>
        [Server]
        public void CheckAllPlayersReady()
        {
            print(playerLobbyHandlers.Count);

            // Check each player's ready state
            foreach (var player in playerLobbyHandlers)
            {
                print(player);
                if (!player.isReady)
                {
                    RpcSetPlayButtonInteractable(false); // Disable the play button if any player is not ready
                    return;
                }
            }

            RpcSetPlayButtonInteractable(true); // Enable the play button if all players are ready
        }

        /// <summary>
        /// Sets the interactable state of the play button on all clients.
        /// </summary>
        /// <param name="truthStatus">True to enable the button, false to disable it</param>
        [ClientRpc]
        void RpcSetPlayButtonInteractable(bool truthStatus)
        {
            playGameButton.interactable = truthStatus;
        }

        /// <summary>
        /// Coroutine to retry updating the player lobby UI after a delay.
        /// </summary>
        private IEnumerator RetryUpdate()
        {
            yield return new WaitForSeconds(1f);
            UpdatePlayerLobbyUI();
        }
    }
}
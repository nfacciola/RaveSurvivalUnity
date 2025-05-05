using Mirror;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RaveSurvival
{
    public class PlayerLobbyHandler : NetworkBehaviour
    {
        // SyncVar to synchronize the player's ready status across the network
        // The hook method is called whenever the value changes
        [SyncVar(hook = nameof(OnReadyStatusChanged))]
        public bool isReady = false;

        // Reference to the ready button in the UI
        public Button readyButton;

        // Reference to the TextMeshProUGUI component for displaying the player's name
        public TextMeshProUGUI nameText;

        /// <summary>
        /// Unity's Start method, called before the first frame update.
        /// Sets the ready button to be interactable only for the local player.
        /// </summary>
        void Start()
        {
            readyButton.interactable = isLocalPlayer;
        }

        /// <summary>
        /// Called when the local player is initialized.
        /// Makes the ready button interactable and sets the initial ready status to false.
        /// </summary>
        public override void OnStartLocalPlayer()
        {
            base.OnStartLocalPlayer();
            readyButton.interactable = true;
            isReady = false;
        }

        /// <summary>
        /// Called when the client starts.
        /// Registers the player with the LobbyUIManager.
        /// </summary>
        public override void OnStartClient()
        {
            base.OnStartClient();
            LobbyUIManager.Instance.RegisterPlayer(this);
        }

        /// <summary>
        /// Command method executed on the server to toggle the player's ready status.
        /// </summary>
        [Command]
        void CmdSetReady()
        {
            isReady = !isReady; // Toggle the ready status
            OnReadyStatusChanged(!isReady, isReady); // Trigger the hook method
        }

        /// <summary>
        /// Called when the ready button is clicked.
        /// Sends a command to the server to toggle the ready status.
        /// </summary>
        public void OnReadyButtonClicked()
        {
            CmdSetReady();
        }

        /// <summary>
        /// Updates the button's color to reflect the player's ready status.
        /// </summary>
        /// <param name="color">The color to set for the button</param>
        void SetSelectedButtonColor(Color color)
        {
            ColorBlock cb = readyButton.colors;
            cb.normalColor = color;
            cb.selectedColor = color;
            cb.disabledColor = color;
            readyButton.colors = cb;
        }

        /// <summary>
        /// Hook method called when the ready status changes.
        /// Updates the UI and checks if all players are ready.
        /// </summary>
        /// <param name="oldValue">The previous ready status</param>
        /// <param name="newValue">The new ready status</param>
        void OnReadyStatusChanged(bool oldValue, bool newValue)
        {
            // If the server is active, check if all players are ready
            if (NetworkServer.active)
            {
                LobbyUIManager.Instance.CheckAllPlayersReady();
            }

            // Update the button color based on the ready status
            if (isReady)
            {
                SetSelectedButtonColor(Color.green); // Green for ready
            }
            else
            {
                SetSelectedButtonColor(Color.white); // White for not ready
            }
        }
    }
}

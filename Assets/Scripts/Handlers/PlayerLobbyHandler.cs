using Mirror;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RaveSurvival
{
    public class PlayerLobbyHandler : NetworkBehaviour
    {
        [SyncVar(hook = nameof(OnReadyStatusChanged))]
        public bool isReady = false;
        public Button readyButton;
        public TextMeshProUGUI nameText;

        void Start()
        {
            readyButton.interactable = false;
        }

        public override void OnStartLocalPlayer()
        {
            base.OnStartLocalPlayer();
        }

        public override void OnStartClient()
        {
            base.OnStartClient();
            LobbyUIManager.Instance.RegisterPlayer(this);
            readyButton.interactable = true;
            isReady = false;
            SetButtonColor(Color.gray);
        }

        [Command]
        void CmdSetReady()
        {
            isReady = !isReady;
            OnReadyStatusChanged(!isReady, isReady);
        }

        public void OnReadyButtonClicked()
        {
            CmdSetReady();
        }

        void SetButtonColor(Color _color)
        {
            var colors = readyButton.colors;
            colors.normalColor = _color;
            readyButton.colors = colors;
        }

        void OnReadyStatusChanged(bool oldValue, bool newValue)
        {
            if (isReady)
            {
                SetButtonColor(Color.green);
            }
            else
            {
                SetButtonColor(Color.gray);
            }

            if(NetworkServer.active)
            {
                LobbyUIManager.Instance.CheckAllPlayersReady();
            }
        }
    }

}

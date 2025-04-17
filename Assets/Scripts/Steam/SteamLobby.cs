using UnityEngine;
using Mirror;
using Steamworks;
using Mirror.Transports.Encryption;
using Edgegap;
using System.Collections;
using Unity.VisualScripting;
using System.Collections.Generic;

public class SteamLobby : NetworkBehaviour
{
    public static SteamLobby Instance;
    public GameObject hostButton = null;
    public ulong lobbyID;
    private NetworkManager networkManager;
    public PanelSwapper panelSwapper;

    protected Callback<LobbyCreated_t> lobbyCreated;
    protected Callback<GameLobbyJoinRequested_t> gameLobbyJoinRequested;
    protected Callback<LobbyEnter_t> lobbyEntered;
    protected Callback<LobbyChatUpdate_t> lobbyChatUpdate;


    private const string HostAddressKey = "HostAddress";

    void Awake()
    {
        Debug.Log("SteamLobby Awake()");
        if (Instance == null)
        {
            Instance = this;
            Debug.Log("SteamLobby Instance assigned");
        }
        else if (Instance != this)
        {
            Debug.LogWarning("Duplicate SteamLobby, destroying this one.");
            Destroy(gameObject);
        }
    }

    void Start()
    {
        networkManager = GetComponent<NetworkManager>();
        if(!SteamManager.Initialized)
        {
            Debug.LogError("Steam is not initialized. Make sure to run this in the Steam environment.");
            return;
        }
        //idk why but the UI manager goes inactive wihout this line
        panelSwapper.gameObject.SetActive(true);
        Debug.Log("SteamLobby Start() running. IsServer: " + NetworkServer.active + ", IsClient: " + NetworkClient.active);
        lobbyCreated = Callback<LobbyCreated_t>.Create(OnLobbyCreated);
        gameLobbyJoinRequested = Callback<GameLobbyJoinRequested_t>.Create(OnGameLobbyJoinRequested);
        lobbyEntered = Callback<LobbyEnter_t>.Create(OnLobbyEntered);
        lobbyChatUpdate = Callback<LobbyChatUpdate_t>.Create(OnLobbyChatUpdate);
    }

    public void HostLobby(){
        SteamMatchmaking.CreateLobby(ELobbyType.k_ELobbyTypeFriendsOnly, networkManager.maxConnections);
    }

    void OnLobbyCreated(LobbyCreated_t callback)
    {
        if (callback.m_eResult != EResult.k_EResultOK)
        {
            Debug.LogError("Failed to create lobby: " + callback.m_eResult);
            return;
        }

        Debug.Log("Lobby created successfully. Lobby ID: " + callback.m_ulSteamIDLobby);
        networkManager.StartHost();

        SteamMatchmaking.SetLobbyData(new CSteamID(callback.m_ulSteamIDLobby), HostAddressKey, SteamUser.GetSteamID().ToString());
        lobbyID = callback.m_ulSteamIDLobby;
        //LobbyUIManager.Instance.CreateLobby();
    }

    void OnGameLobbyJoinRequested(GameLobbyJoinRequested_t callback)
    {
        Debug.Log("Join request received for lobby: " + callback.m_steamIDLobby);
        SteamMatchmaking.JoinLobby(callback.m_steamIDLobby);
    }

    void OnLobbyChatUpdate(LobbyChatUpdate_t callback)
    {
        if (callback.m_ulSteamIDLobby != lobbyID) return;

        EChatMemberStateChange stateChange = (EChatMemberStateChange)callback.m_rgfChatMemberStateChange;
        Debug.Log($"LobbyChatUpdate: {stateChange}");

        bool shouldUpdate = stateChange.HasFlag(EChatMemberStateChange.k_EChatMemberStateChangeEntered) ||
                        stateChange.HasFlag(EChatMemberStateChange.k_EChatMemberStateChangeLeft) ||
                        stateChange.HasFlag(EChatMemberStateChange.k_EChatMemberStateChangeDisconnected) ||
                        stateChange.HasFlag(EChatMemberStateChange.k_EChatMemberStateChangeKicked) ||
                        stateChange.HasFlag(EChatMemberStateChange.k_EChatMemberStateChangeBanned);

        if (shouldUpdate)
        {
            StartCoroutine(DelayedNameUpdate(0.5f));
        }
    }

    private IEnumerator DelayedHostPromotion(float delay)
    {
        yield return new WaitForSeconds(delay);

        CSteamID currentOwner = SteamMatchmaking.GetLobbyOwner(new CSteamID(lobbyID));
        CSteamID me = SteamUser.GetSteamID();

        if (currentOwner == me && !NetworkServer.active)
        {
            Debug.Log("I am confirmed as new host after delay. Starting host server...");
            networkManager.StartHost();
            SteamMatchmaking.SetLobbyData(new CSteamID(lobbyID), HostAddressKey, me.ToString());
        }
    }

    private IEnumerator DelayedNameUpdate(float delay)
    {
        if (LobbyUIManager.Instance == null)
        {
            Debug.LogWarning("LobbyUIManager.Instance is null, skipping name update.");
            yield break;
        }
        yield return new WaitForSeconds(delay);
        LobbyUIManager.Instance?.UpdatePlayerNames();
    }

    void OnLobbyEntered(LobbyEnter_t callback)
    {
        if(NetworkServer.active)
        {
            Debug.Log("Already in a lobby as host. Ignoring join request.");
            return;
        }
        lobbyID = callback.m_ulSteamIDLobby;
        string hostAddress = SteamMatchmaking.GetLobbyData(new CSteamID(callback.m_ulSteamIDLobby), HostAddressKey);
        networkManager.networkAddress = hostAddress;
        Debug.Log("Entered lobby: " + callback.m_ulSteamIDLobby);
        networkManager.StartClient();
        panelSwapper.SetActivePanel(1); // Assuming 1 is the index for the lobby panel
        //LobbyUIManager.Instance?.UpdatePlayerNames();
        //StartCoroutine(DelayedNameUpdate(.5f));
    }


    public void LeaveLobby()
    {
      CSteamID currentOwner = SteamMatchmaking.GetLobbyOwner(new CSteamID(lobbyID));
      CSteamID me = SteamUser.GetSteamID();
      var lobby = new CSteamID(lobbyID);
      List<CSteamID> members = new List<CSteamID>();

      int count = SteamMatchmaking.GetNumLobbyMembers(lobby);

      for(int i = 0; i < count; i++) {
        members.Add(SteamMatchmaking.GetLobbyMemberByIndex(lobby, i));
      }

      Debug.Log("Player Leaving Lobby");

        if(lobbyID != 0)
        {
            SteamMatchmaking.LeaveLobby(new CSteamID(lobbyID));
            lobbyID = 0;
        }

        if(NetworkServer.active && currentOwner == me)
        {
            NetworkManager.singleton.StopHost();
        }
        else if(NetworkClient.isConnected)
        {
            NetworkManager.singleton.StopClient();
        }

        
    }
}

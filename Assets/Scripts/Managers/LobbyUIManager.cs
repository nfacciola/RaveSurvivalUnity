using Steamworks;
using TMPro;
using Mirror;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System;
using Unity.VisualScripting;

public class LobbyUIManager : NetworkBehaviour
{
    public static LobbyUIManager Instance;
    public Transform playerListParent;
    public List<TextMeshProUGUI> playerNameTexts = new List<TextMeshProUGUI>();
    public List<PlayerLobbyHandler> playerLobbyHandlers = new List<PlayerLobbyHandler>();
    public Button playGameButton;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {

            Destroy(gameObject);
            return;
        }
    }

    void Start()
    {
        playGameButton.interactable = false;
    }

    public void UpdatePlayerNames()
    {
        playerNameTexts.Clear();
        var lobby = new CSteamID(SteamLobby.Instance.lobbyID);
        Debug.Log($"Get Lobby ID: {SteamLobby.Instance.lobbyID}");
        int memberCount = SteamMatchmaking.GetNumLobbyMembers(lobby);
        Debug.Log($"Updating player names. Lobby member count: {memberCount}");

        CSteamID hostID = new CSteamID(ulong.Parse(SteamMatchmaking.GetLobbyData(lobby, "HostAddress")));
        List<CSteamID> orderedMembers = new List<CSteamID>();

        if (memberCount == 0)
        {
            Debug.LogWarning("Lobby has no members yet, retrying...");
            StartCoroutine(RetryUpdate());
            return;
        }

        ///Show host first on every machine
        // First: Add host
        orderedMembers.Add(hostID);
        // Then: Add everyone else except host
        for (int i = 0; i < memberCount; i++)
        {
            CSteamID memberID = SteamMatchmaking.GetLobbyMemberByIndex(lobby, i);
            if (memberID != hostID)
            {
                orderedMembers.Add(memberID);
            }
        }

        int j = 0;
        foreach(var member in orderedMembers)
        {  
            TextMeshProUGUI txtMesh = playerListParent.GetChild(j).GetChild(0).GetComponent<TextMeshProUGUI>();
            playerNameTexts.Add(txtMesh);
            print(SteamFriends.GetFriendPersonaName(member));
            string playerName = SteamFriends.GetFriendPersonaName(member);
            playerNameTexts[j].text = playerName;
            j++;
        }
    }

    public void RegisterPlayer(PlayerLobbyHandler player)
    {
        player.transform.SetParent(playerListParent, false);
        //playerLobbyHandlers.Add(player);
        //playerNameTexts.Add(textMesh);
        UpdatePlayerNames();
    }

    [Server]
    public void CheckAllPlayersReady()
    {
        foreach(var player in playerLobbyHandlers)
        {
            if(!player.isReady)
                return;
        }
        RpcEnablePlayButton();
    }

    [ClientRpc]
    void RpcEnablePlayButton()
    {
        playGameButton.interactable = true;
    }

    private IEnumerator RetryUpdate()
    {
        yield return new WaitForSeconds(1f);
        UpdatePlayerNames();
    }

}

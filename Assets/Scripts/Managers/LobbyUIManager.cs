using Steamworks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class LobbyUIManager : MonoBehaviour
{
    public static LobbyUIManager Instance;
    public TextMeshProUGUI[] playerNameTexts;

    void Awake()
    {
        Instance = Instance == null ? this : Instance;
        if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    public void UpdatePlayerNames()
    {
        var lobby = new CSteamID(SteamLobby.Instance.lobbyID);
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

        //Show names
        for (int i = 0; i < playerNameTexts.Length; i++)
        {
            string playerName = i < memberCount ? SteamFriends.GetFriendPersonaName(orderedMembers[i]) : "";
            playerNameTexts[i].text = playerName;
        }
    }

    private IEnumerator RetryUpdate()
    {
        yield return new WaitForSeconds(1f);
        UpdatePlayerNames();
    }

}

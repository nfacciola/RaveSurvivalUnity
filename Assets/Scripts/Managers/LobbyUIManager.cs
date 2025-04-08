using Steamworks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

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

        if (memberCount == 0)
        {
            Debug.LogWarning("Lobby has no members yet, retrying...");
            StartCoroutine(RetryUpdate());
            return;
        }

        for (int i = 0; i < playerNameTexts.Length; i++)
        {
            if (i < memberCount)
            {
                CSteamID playerID = SteamMatchmaking.GetLobbyMemberByIndex(lobby, i);
                string playerName = SteamFriends.GetFriendPersonaName(playerID);
                Debug.Log($"Player {i}: {playerName}");
                playerNameTexts[i].text = playerName;
            }
            else
            {
                playerNameTexts[i].text = "";
            }
        }
    }

    private IEnumerator RetryUpdate()
    {
        yield return new WaitForSeconds(1f);
        UpdatePlayerNames();
    }

}

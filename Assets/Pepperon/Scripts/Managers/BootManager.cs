using System;
using System.Collections.Generic;
using Mirror;
using Newtonsoft.Json;
using Pepperon.Scripts.UI;
using UnityEngine;

namespace Pepperon.Scripts.Managers {
public class BootManager : MonoBehaviour {
    public static BootManager Instance { get; private set; }

    public string MatchId { get; private set; }
    public LobbyResponse Lobby { get; private set; }
    public bool IsBot { get; private set; }
    public event Action OnServerInit;
    public event Action OnClientInit;

    private void Awake() {
        Instance = this;
    }

    private void Start() {
        if (IsServer()) {
            InitServer();
            OnServerInit?.Invoke();
        } else {
            InitClient();
            OnClientInit?.Invoke();
        }
    }

    private static bool IsServer() {
        var isServer = Environment.GetEnvironmentVariable("IS_SERVER") != null;
        Debug.Log("Is server: " + isServer);
        return isServer;
    }

    private void InitServer() {
        Debug.Log("Init server");

        MatchId = Environment.GetEnvironmentVariable("MATCH_ID");
        var lobbyJson = Environment.GetEnvironmentVariable("LOBBY");
        if (!string.IsNullOrEmpty(lobbyJson))
            Lobby = JsonConvert.DeserializeObject<LobbyResponse>(lobbyJson);

        NetworkManager.singleton.StartServer();
    }

    private void InitClient() {
        Debug.Log("Init client");

        IsBot = Environment.GetEnvironmentVariable("IS_BOT") != null;
        if (IsBot) {
            PlayerPrefs.SetString("MatchAddress", "match-" + Environment.GetEnvironmentVariable("MATCH_ID"));
            PlayerPrefs.SetString("PlayerId", Environment.GetEnvironmentVariable("PLAYER_ID"));
        }

        Debug.Log("InitClient: Is bot - " + IsBot);

        var address = PlayerPrefs.GetString("MatchAddress", "localhost");
        NetworkManager.singleton.networkAddress = address;

        NetworkManager.singleton.StartClient();
    }
}
}
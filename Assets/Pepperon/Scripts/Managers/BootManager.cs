using System;
using Mirror;
using Newtonsoft.Json;
using Pepperon.Scripts.UI;
using UnityEngine;

namespace Pepperon.Scripts.Managers {
public class BootManager : MonoBehaviour {
    public static BootManager Instance { get; private set; }

    public string MatchId { get; private set; }
    public LobbyResponse Lobby { get; private set; }

    private void Awake() {
        Instance = this;
    }

    private void Start() {
        if (IsServer())
            InitServer();
        else
            InitClient();
    }

    private bool IsServer() {
        var dedicatedServer = Application.isBatchMode;
        Debug.Log("Is server: " + dedicatedServer);
        return dedicatedServer;
    }
    
    private void InitServer() {
        Debug.Log("Init server");
        MatchId = Environment.GetEnvironmentVariable("MATCH_ID");
        string lobbyJson = Environment.GetEnvironmentVariable("LOBBY");
        if (!string.IsNullOrEmpty(lobbyJson)) {
            Lobby = JsonConvert.DeserializeObject<LobbyResponse>(lobbyJson);
        }
        NetworkManager.singleton.StartServer();
    }
    
    private void InitClient() {
        Debug.Log("Init client");
        var address = PlayerPrefs.GetString("MatchAddress", "localhost");
        NetworkManager.singleton.networkAddress = address;
        NetworkManager.singleton.StartClient();
    }
}
}
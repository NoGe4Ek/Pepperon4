using System;
using System.Collections.Generic;
using System.Linq;
using kcp2k;
using Mirror;
using Mirror.SimpleWeb;
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
        }
        else {
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
        var portString = Environment.GetEnvironmentVariable("PORT");
        ushort.TryParse(portString, out var port);
        var lobbyJson = Environment.GetEnvironmentVariable("LOBBY");
        if (!string.IsNullOrEmpty(lobbyJson))
            Lobby = JsonConvert.DeserializeObject<LobbyResponse>(lobbyJson);

        ((NetworkManager.singleton.transport as MultiplexTransport).transports.First(it => it is KcpTransport) as
            KcpTransport).port = port;

        NetworkManager.singleton.StartServer();
    }

    private void InitClient() {
        Debug.Log("Init client");
        
        IsBot = Environment.GetEnvironmentVariable("IS_BOT") != null;
        Debug.Log("InitClient: Is bot - " + IsBot);
        if (IsBot) {
            PlayerPrefs.SetString("Port", Environment.GetEnvironmentVariable("PORT"));
            PlayerPrefs.SetString("MatchAddress", "match-" + Environment.GetEnvironmentVariable("MATCH_ID"));
            PlayerPrefs.SetString("PlayerId", Environment.GetEnvironmentVariable("PLAYER_ID"));
        }

        var portString = PlayerPrefs.GetString("Port");
        ushort.TryParse(portString, out var port);
        var matchAddress = PlayerPrefs.GetString("MatchAddress");
        var playerId = PlayerPrefs.GetString("PlayerId");
        var address = PlayerPrefs.GetString("MatchAddress", "localhost");

        string scheme = "";
        UriBuilder uriBuilder = new UriBuilder();
        if (NetworkManager.singleton.transport is MultiplexTransport multiplexTransport) {
            foreach (var t in multiplexTransport.transports) {
                if (t is KcpTransport kcp && kcp.Available()) {
                    scheme = KcpTransport.Scheme;
                    uriBuilder.Scheme = scheme;
                    uriBuilder.Host = $"{portString}.aphirri.ru";
                    uriBuilder.Port = port;
                    
                    NetworkManager.singleton.networkAddress = $"{portString}.aphirri.ru";
                    ((NetworkManager.singleton.transport as MultiplexTransport).transports
                        .First(it => it is KcpTransport) as KcpTransport).port = port;
                    break;
                }

                if (t is SimpleWebTransport swt && swt.Available()) {
                    scheme = (swt.sslEnabled || swt.clientUseWss) ? SimpleWebTransport.SecureScheme : SimpleWebTransport.NormalScheme;
                    uriBuilder.Scheme = scheme;
                    uriBuilder.Host = $"www.aphirri.ru";
                    uriBuilder.Port = swt.clientUseWss ? 443 : 80;
                    uriBuilder.Path = "match";
                    uriBuilder.Query = $"port={Uri.EscapeDataString(portString)}";
                    
                    NetworkManager.singleton.networkAddress = "213.21.27.138";
                    ((NetworkManager.singleton.transport as MultiplexTransport).transports.First(it => it is SimpleWebTransport) as
                        SimpleWebTransport).port = swt.clientUseWss ? (ushort)443 : (ushort)80;
                }
            }
        }
        
        var uri = uriBuilder.Uri;
        // Логируем все компоненты Uri
        Debug.Log("===== URI COMPONENTS =====");
        Debug.Log($"AbsoluteUri: {uri.AbsoluteUri}");
        Debug.Log($"Scheme: {uri.Scheme}");
        Debug.Log($"Host: {uri.Host}");
        Debug.Log($"Port: {uri.Port}");
        Debug.Log($"AbsolutePath: {uri.AbsolutePath}");
        Debug.Log($"PathAndQuery: {uri.PathAndQuery}");
        Debug.Log($"Query: {uri.Query}");
        Debug.Log($"Fragment: {uri.Fragment}");
        Debug.Log($"UserInfo: {uri.UserInfo}");
        Debug.Log($"DnsSafeHost: {uri.DnsSafeHost}");
        Debug.Log($"IsDefaultPort: {uri.IsDefaultPort}");
        Debug.Log($"IsFile: {uri.IsFile}");
        Debug.Log($"IsLoopback: {uri.IsLoopback}");
        Debug.Log($"IsUnc: {uri.IsUnc}");
        Debug.Log($"OriginalString: {uri.OriginalString}");
        Debug.Log("==========================");
        NetworkManager.singleton.StartClient(uri);
    }
}
}
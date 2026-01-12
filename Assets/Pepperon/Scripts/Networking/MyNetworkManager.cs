using System.Linq;
using Mirror;
using Pepperon.Scripts.Controllers;
using Pepperon.Scripts.Entities.Systems.LoreSystem.Base;
using Pepperon.Scripts.Managers;
using UnityEngine;
using Race = Pepperon.Scripts.UI.Race;

namespace Pepperon.Scripts.Networking {
public class MyNetworkManager : NetworkManager {
    // [Server]
    public override void OnServerAddPlayer(NetworkConnectionToClient conn) {
        Debug.Log("New player connected: " + conn);

        GameObject player = Instantiate(playerPrefab);
        player.name = $"{playerPrefab.name} [connId={conn.connectionId}]";
        NetworkServer.AddPlayerForConnection(conn, player);

        // PlayerController playerController = player.GetComponent<PlayerController>();
        // SessionManager.Instance.AddPlayer(conn, playerController);
    }

    // [Server]
    public override void OnServerDisconnect(NetworkConnectionToClient conn) {
        // base.OnServerDisconnect(conn);
        // SessionManager.Instance.RemovePlayer(conn);
    }
}
}
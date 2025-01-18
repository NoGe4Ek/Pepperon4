using Mirror;
using Pepperon.Scripts.Controllers;
using Pepperon.Scripts.Managers;
using UnityEngine;

namespace Pepperon.Scripts.Networking {
public class MyNetworkManager : NetworkManager {
    public override void OnServerAddPlayer(NetworkConnectionToClient conn) {
        GameObject player = Instantiate(playerPrefab);
        PlayerController playerController = player.GetComponent<PlayerController>();
        SessionManager.Instance.players.Add(conn, playerController);
        
        player.name = $"{playerPrefab.name} [connId={conn.connectionId}]";

        NetworkServer.AddPlayerForConnection(conn, player);
    }
}
}
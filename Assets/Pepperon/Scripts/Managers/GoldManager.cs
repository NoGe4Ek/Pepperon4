using System;
using Mirror;
using Pepperon.Scripts.Controllers;
using UnityEngine;

namespace Pepperon.Scripts.Managers {
public class GoldManager : NetworkBehaviour {
    public static GoldManager Instance { get; private set; }

    private void Awake() {
        Instance = this;
    }

    private void Start() {
        if (!isServer) return;
        BattleManager.OnKill += OnKill;
    }

    private static void OnKill(GameObject killerObject, GameObject diedObject) {
        killerObject.GetComponent<NetworkIdentity>().connectionToClient.identity
            .GetComponent<PlayerController>().gold += 10;
    }
}
}
using System;
using Mirror;
using Pepperon.Scripts.Controllers;
using UnityEngine;
using UnityEngine.UI;

namespace Pepperon.Scripts.Managers {
public class UIManager : NetworkBehaviour {
    public Button upgradeDamageButton;
    private UpgradeManager upgradeManager;

    private void Start() {
        upgradeManager = FindObjectOfType<UpgradeManager>();
        upgradeDamageButton.onClick.AddListener(OnUpgradeDamageClick);
    }

    [Client]
    private void OnUpgradeDamageClick() {
        if (!isClient) return;
        // Debug.Log("connectionToClient: " + NetworkClient.localPlayer.connectionToClient.connectionId);
        Debug.Log("connectionToServer: " + NetworkClient.localPlayer.connectionToServer.connectionId);
        PlayerController.localPlayer.CmdUpgradeMeleeDamage();
    }
}
}
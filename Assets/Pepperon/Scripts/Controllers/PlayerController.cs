using System;
using System.Collections.Generic;
using System.Linq;
using Mirror;
using Pepperon.Scripts.Entities.Systems.LoreSystem;
using Pepperon.Scripts.Entities.Systems.LoreSystem.Infos;
using Pepperon.Scripts.Entities.Systems.LoreSystem.Upgrades;
using Pepperon.Scripts.Managers;
using UnityEngine;

namespace Pepperon.Scripts.Controllers {
public class PlayerController : NetworkBehaviour {
    // non session info
    public string username;
    public int rating;

    [SyncVar] public int gold;
    [SyncVar] public Race race;
    public int income;


    // public Dictionary<CommonUpgradeType, int> upgrades =
    //     new List<CommonUpgradeType>((CommonUpgradeType[])Enum.GetValues(typeof(CommonUpgradeType))).ToDictionary(
    //         upgradeType => upgradeType, _ => 0);


    public int playerType;
    public GameObject mainBuilding;

    public static PlayerController localPlayer;

    public override void OnStartLocalPlayer() {
        base.OnStartLocalPlayer();
        localPlayer = this;
    }

    [Command]
    public void CmdUpgradeMeleeDamage() {
        Debug.Log("Upgrade melee unit. connectionToClient = " + connectionToClient);
        var player = SessionManager.Instance.players[connectionToClient];
        var upgrade = player.race.commonUpgrades[CommonUpgradeType.Melee];
        upgrade.progress++;
        var upgradeDelta = upgrade.progressDeltas[upgrade.progress];
        player.race.units
            .Where(unit =>
                unit.commonUpgradeTypes
                    .Any(uType =>
                        uType == CommonUpgradeType.Melee)
            )
            .ToList()
            .ForEach(unit =>
                unit.info.OfType<AttackingInfo>()
                    .First().attack += upgradeDelta
            );
    }

    // [ClientRpc]
    // public void RpcUpgradeMeleeDamage() {
    //     if (isServer) return;
    //     
    // }
}
}
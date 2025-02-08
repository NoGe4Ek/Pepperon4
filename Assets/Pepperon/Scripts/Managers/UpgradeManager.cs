using System;
using System.Linq;
using Mirror;
using Pepperon.Scripts.Controllers;
using Pepperon.Scripts.Entities.Systems.LoreSystem.Base.Entities;
using Pepperon.Scripts.Entities.Systems.LoreSystem.Base.Infos;
using Pepperon.Scripts.Entities.Systems.LoreSystem.Base.Upgrades;
using UnityEngine;

namespace Pepperon.Scripts.Managers {
public class UpgradeManager : NetworkBehaviour {
    public static UpgradeManager Instance { get; private set; }

    private void Awake() {
        Instance = this;
    }

    [Command(requiresAuthority = false)]
    public void CmdUpgrade(int playerId, CommonUpgradeType upgradeType) {
        var player = SessionManager.Instance.knownPlayers[playerId];
        var upgradeProgress = player.progress.upgrades[upgradeType];
        var upgrade = player.race.upgrades[upgradeType];

        if (upgradeProgress.progress + 1 > upgrade.progressCosts.Length - 1) {
            player.SendAlert(playerId, "No more upgrades");
            return;
        }

        if (player.gold < upgrade.progressCosts[upgradeProgress.progress + 1]) {
            player.SendAlert(playerId,
                "Not enough gold for upgrade. Need: + " + upgrade.progressCosts[upgradeProgress.progress + 1]);
            return;
        }

        Upgrade(playerId, upgradeType);
        RpcUpgrade(playerId, upgradeType);
    }

    [ClientRpc]
    private void RpcUpgrade(int playerId, CommonUpgradeType upgradeType) {
        // dedicated if (isServer) return;
        Upgrade(playerId, upgradeType);
    }

    private void Upgrade(int playerId, CommonUpgradeType upgradeType) {
        CommonUnitsUpgrade(
            playerId,
            upgradeType,
            GetUpgradeAction(playerId, upgradeType)
        );
    }

    private Action<EntityProgress, UpgradeProgress, Upgrade> GetUpgradeAction(int playerId,
        CommonUpgradeType upgradeType) {
        var player = SessionManager.Instance.knownPlayers[playerId];

        switch (upgradeType) {
            case CommonUpgradeType.Melee:
                return (unit, upgradeProgress, upgrade) => {
                    unit.info.OfType<AttackingInfoProgress>()
                        .First().attack += upgrade.progressDeltas[upgradeProgress.progress];
                };
            case CommonUpgradeType.Range:
                return (unit, upgradeProgress, upgrade) => {
                    unit.info.OfType<AttackingInfoProgress>()
                        .First().attack += upgrade.progressDeltas[upgradeProgress.progress];
                };
            case CommonUpgradeType.Survivability:
                return (unit, upgradeProgress, upgrade) => {
                    unit.info.OfType<SurvivabilityInfoProgress>()
                        .First().maxHealth += upgrade.progressDeltas[upgradeProgress.progress];
                };

            default:
                return null;
        }
    }

    private void CommonUnitsUpgrade(int playerId, CommonUpgradeType upgradeType,
        Action<EntityProgress, UpgradeProgress, Upgrade> upgradeAction) {
        var player = SessionManager.Instance.knownPlayers[playerId];
        var upgradeProgress = player.progress.upgrades[upgradeType];
        var upgrade = player.race.upgrades[upgradeType];
        upgradeProgress.progress++;
        if (isServer)
            player.gold -= upgrade.progressCosts[upgradeProgress.progress];

        player.progress.entities[CommonEntityType.Units]
            .Where(unit =>
                unit.commonUpgradeTypes
                    .Any(uType =>
                        uType == upgradeType)
            )
            .ToList()
            .ForEach(unit =>
                upgradeAction(unit, upgradeProgress, upgrade)
            );
    }
}
}
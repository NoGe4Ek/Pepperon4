using System;
using System.Linq;
using Mirror;
using Pepperon.Scripts.Entities.Systems.LoreSystem.Base;
using Pepperon.Scripts.Entities.Systems.LoreSystem.Base.Entities;
using Pepperon.Scripts.Entities.Systems.LoreSystem.Base.Infos;
using Pepperon.Scripts.Entities.Systems.LoreSystem.Base.Upgrades;
using Pepperon.Scripts.Managers;
using UnityEngine;

namespace Pepperon.Scripts.Controllers {
public class PlayerController : NetworkBehaviour {
    [SyncVar] public int gold;
    [SerializeField] public RaceProgress progress;

    // not use sync var to avoid sending null at start
    public Race race;
    public void SetRace(Race newRace) {
        race = newRace;
        progress = newRace.ToProgress();
        RpcSetRace(newRace);
    }
    [ClientRpc]
    private void RpcSetRace(Race newRace) {
        if (isServer) return;
        race = newRace;
        progress = newRace.ToProgress();
    }

    public int playerId;

    public void SetPlayerId(int newPlayerId) {
        playerId = newPlayerId;
        SessionManager.Instance.AddKnownPlayer(newPlayerId);
        RpcSetPlayerId(newPlayerId);
    }
    [ClientRpc]
    private void RpcSetPlayerId(int newPlayerId) {
        if (isServer) return;
        playerId = newPlayerId;
        SessionManager.Instance.AddKnownPlayer(newPlayerId);
    }

    public GameObject mainBuilding;

    public static PlayerController localPlayer;

    private void Awake() {
        playerId = -1;
    }

    public override void OnStartLocalPlayer() {
        base.OnStartLocalPlayer();
        localPlayer = this;
    }

    [Command]
    public void CmdUpgradeMeleeDamage() {
        Debug.Log("Upgrade melee unit. connectionToClient = " + connectionToClient);
        UpgradeMeleeDamage();
        RpcUpgradeMeleeDamage();
    }

    private void UpgradeMeleeDamage() {
        var player = SessionManager.Instance.knownPlayers[playerId];
        var upgrade = player.progress.upgrades[CommonUpgradeType.Melee];
        upgrade.progress++;
        var upgradeDelta = player.race.upgrades[CommonUpgradeType.Melee].progressDeltas[upgrade.progress];

        player.progress.entities[CommonEntityType.Units]
            .Where(unit =>
                unit.commonUpgradeTypes
                    .Any(uType =>
                        uType == CommonUpgradeType.Melee)
            )
            .ToList()
            .ForEach(unit =>
                unit.info.OfType<AttackingInfoProgress>()
                    .First().attack += upgradeDelta
            );
    }

    [ClientRpc]
    public void RpcUpgradeMeleeDamage() {
        if (isServer) return;
        UpgradeMeleeDamage();
    }
}
}
using System;
using System.Linq;
using Mirror;
using Pepperon.Scripts.Controllers;
using Pepperon.Scripts.Entities.Systems.LoreSystem.Base;
using Pepperon.Scripts.Managers;
using Pepperon.Scripts.Systems.LoreSystem.Base;
using Pepperon.Scripts.Systems.LoreSystem.Base.Entities;
using UnityEngine;
using UnityEngine.UI;

namespace Pepperon.Scripts.Entities.Controllers {
public class EntityController : NetworkBehaviour {
    [SyncVar] public EntityId entityId;
    [SyncVar(hook = nameof(OnPlayerTypeChange))] public int playerType;

    private void Awake() {
        playerType = -1;
    }

    private void OnPlayerTypeChange(int oldPlayerType, int newPlayerType) {
        Image[] images = gameObject.GetComponentsInChildren<Image>();
        Image specificImage = images.FirstOrDefault(image => image.name == "Fill");
        if (specificImage)
            specificImage.color = newPlayerType == PlayerController.localPlayer.playerId
                ? Color.green
                : Color.red;
    }

    public Entity entity =>
        isServer
            ? SessionManager.Instance.players[connectionToClient].race.entities[entityId.entityType]
                [entityId.entityIndex]
            : PlayerController.localPlayer.race.entities[entityId.entityType][entityId.entityIndex];


    public EntityProgress entityProgress() {
        Debug.Log(
            "ENTITY PROGRESS: PlayerId - " + PlayerController.localPlayer.playerId +
            ", Progress - " + PlayerController.localPlayer.progress.GetHashCode() +
            ", Local player - " + PlayerController.localPlayer.GetHashCode()
            );
        if (isServer)
            return SessionManager.Instance.players[connectionToClient].progress.entities[entityId.entityType]
                [entityId.entityIndex];
        if (isOwned) {
            return PlayerController.localPlayer.progress.entities[entityId.entityType][entityId.entityIndex];
        }

        return SessionManager.Instance.knownPlayers[playerType].progress.entities[entityId.entityType][entityId.entityIndex];

    }
}
}
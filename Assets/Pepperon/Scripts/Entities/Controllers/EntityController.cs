using Mirror;
using Pepperon.Scripts.Controllers;
using Pepperon.Scripts.Entities.Systems.LoreSystem.Base;
using Pepperon.Scripts.Entities.Systems.LoreSystem.Base.Entities;
using Pepperon.Scripts.Managers;

namespace Pepperon.Scripts.Entities.Controllers {
public class EntityController : NetworkBehaviour {
    [SyncVar] public EntityId entityId;
    public int playerType;

    public Entity entity =>
        isServer
            ? SessionManager.Instance.players[connectionToClient].race.entities[entityId.entityType]
                [entityId.entityIndex]
            : PlayerController.localPlayer.race.entities[entityId.entityType][entityId.entityIndex];
    

    public EntityProgress entityProgress =>
        isServer
            ? SessionManager.Instance.players[connectionToClient].progress.entities[entityId.entityType]
                [entityId.entityIndex]
            : PlayerController.localPlayer.progress.entities[entityId.entityType][entityId.entityIndex];
}
}
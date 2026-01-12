using Mirror;
using Pepperon.Scripts.Controllers;
using Pepperon.Scripts.Managers;
using Pepperon.Scripts.Systems.LoreSystem.Base.Cards;
using Pepperon.Scripts.Systems.LoreSystem.Base.Entities;

namespace Pepperon.Scripts.Entities.Controllers {
public class CardController : NetworkBehaviour {
    [SyncVar] public CardId cardId;

    public BaseCard card =>
        isServer
            ? SessionManager.Instance.players[connectionToClient].race.cards[cardId.rarity][cardId.cardIndex]
            : PlayerController.localPlayer.race.cards[cardId.rarity][cardId.cardIndex];
}
}
using Mirror;
using Pepperon.Scripts.Controllers;
using Pepperon.Scripts.Entities.Controllers;
using Pepperon.Scripts.Managers;
using Pepperon.Scripts.Systems.LoreSystem.Base.Cards;
using UnityEngine;
using UnityEngine.UI;

namespace Pepperon.Scripts.UI.Interaction {
public class BuildingCardDropArea : NetworkBehaviour, IDropCardArea {
    [SerializeField] private Transform boundCardsParent;
    [SerializeField] private GameObject boundCardPrefab;
    public void OnDropCard(CardController cardController) {
        HandManager.Instance.RemoveCard(cardController.gameObject);
        Destroy(cardController.gameObject);
        var boundCard = Instantiate(boundCardPrefab, boundCardsParent);
        boundCard.GetComponent<Image>().sprite = cardController.card.image;
        
        TestRemoveCard(cardController.cardId);
    }
    
    [Command]
    public void TestRemoveCard(CardId cardId) {
        SessionManager.Instance.players[connectionToClient].handCards.Remove(cardId);
        SessionManager.Instance.players[connectionToClient].boundCards.Add(cardId);
    }
}
}
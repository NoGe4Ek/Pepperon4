using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Mirror;
using Pepperon.Scripts.Controllers;
using Pepperon.Scripts.Entities.Controllers;
using Pepperon.Scripts.Systems.LoreSystem.Base.Cards;
using Pepperon.Scripts.UI.Interaction;
using UnityEngine;
using UnityEngine.Splines;
using UnityEngine.UI;
using Random = System.Random;

// Мэнеджер для управления анимациями карт в руке. Часть UI Manager?
namespace Pepperon.Scripts.Managers {
public class HandManager : NetworkBehaviour {
    [SerializeField] private int maxHandSize;
    [SerializeField] private GameObject cardPrefab;
    [SerializeField] private SplineContainer splineContainer;
    [SerializeField] private RectTransform cardsContainer;
    [SerializeField] private Transform spawnPoint;
    private readonly List<GameObject> handCards = new();

    public void RemoveCard(GameObject card) {
        handCards.Remove(card);
        UpdateCardPositions();
    }

    public void AddCard(GameObject card) {
        handCards.Add(card);
        UpdateCardPositions();
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.Space)) BuyEntityCard();
    }

    private void BuyEntityCard() {
        if (handCards.Count >= maxHandSize) return;
        Debug.Log("Buy entity card for playerId: " + PlayerController.localPlayer.playerId);

        CmdBuyEntityCard(PlayerController.localPlayer.playerId);
    }
    
    [Command(requiresAuthority = false)]
    private void CmdBuyEntityCard(int playerId) {
        Debug.Log("CmdBuyEntityCard for playerId: " + playerId);
        
        var player = SessionManager.Instance.knownPlayers[playerId];
        var cardId = DeckManager.Instance.GetRandomCardId(player.level, player.race);
        player.handCards.Add(cardId);
    }

    public void DrawCard(CardId cardId) {
        var card = PlayerController.localPlayer.race.cards[cardId.rarity][cardId.cardIndex];
        
        GameObject g = Instantiate(cardPrefab, spawnPoint.position, Quaternion.identity, cardsContainer);
        g.GetComponentsInChildren<Image>().FirstOrDefault(image => image.name == "Icon").sprite = card.image;
        g.GetComponent<CardController>().cardId = cardId;
        
        AddCard(g);
    }

    private void UpdateCardPositions() {
        if (handCards.Count == 0) return;

        float cardSpacing = 1f / maxHandSize;
        float firstCardPosition = 0.5f - (handCards.Count - 1) * cardSpacing / 2;
        Spline spline = splineContainer.Spline;
        for (var i = 0; i < handCards.Count; i++) {
            float p = firstCardPosition + i * cardSpacing;

            Vector3 splinePosition = spline.EvaluatePosition(p);
            splinePosition.x += (cardsContainer.rect.width / 2);
            splinePosition.y += splineContainer.transform.position.y;
            Vector3 forward = spline.EvaluateTangent(p);
            Vector3 up = spline.EvaluateUpVector(p);

            Quaternion rotation = Quaternion.LookRotation(up, Vector3.Cross(up, forward).normalized);

            // Двигаем только если сейчас с карточкой не взаимодействуют
            if (!handCards[i].GetComponent<CardInteraction>().inProgress) {
                handCards[i].GetComponent<CardInteraction>().inAddingProgress = true;
                var cardIndex = i;
                handCards[i].GetComponent<CardInteraction>().returnMove = handCards[i].transform
                    .DOMove(splinePosition, 0.5f)
                    .OnKill(() => { handCards[cardIndex].GetComponent<CardInteraction>().inAddingProgress = false; })
                    .OnComplete(
                        () => { handCards[cardIndex].GetComponent<CardInteraction>().inAddingProgress = false; });
                handCards[i].GetComponent<CardInteraction>().returnRotation =
                    handCards[i].transform.DOLocalRotateQuaternion(rotation, 0.5f);
            }

            // Если карточки не на своем месте, например, уже перетягиваются, после отпускания
            // должны вернуться на новое актуальное место
            handCards[i].GetComponent<CardInteraction>().originalPosition = splinePosition;
            handCards[i].GetComponent<CardInteraction>().originalRotation = rotation;
        }
    }

    public static HandManager Instance { get; private set; }

    private void Awake() {
        Instance = this;
    }
}
}
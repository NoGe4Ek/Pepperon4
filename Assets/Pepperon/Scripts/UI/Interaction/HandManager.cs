using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Splines;

namespace Pepperon.Scripts.UI.Interaction {
public class HandManager : MonoBehaviour {
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

    private void Update() {
        if (Input.GetKeyDown(KeyCode.Space)) DrawCard();
    }

    private void DrawCard() {
        if (handCards.Count >= maxHandSize) return;

        GameObject g = Instantiate(cardPrefab, spawnPoint.position, Quaternion.identity, cardsContainer);
        handCards.Add(g);
        UpdateCardPositions();
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
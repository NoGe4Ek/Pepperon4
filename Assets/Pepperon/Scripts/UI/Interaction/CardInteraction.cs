using System;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using Pepperon.Scripts.DI;
using Pepperon.Scripts.Entities.Controllers;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Pepperon.Scripts.UI.Interaction {
public class CardInteraction : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler {
    private Collider2D col;
    private Vector3 startDragPosition;


    private Canvas canvas;
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    public Vector3 originalPosition;
    public Quaternion originalRotation;
    public bool inProgress;
    public bool inAddingProgress;
    public TweenerCore<Vector3, Vector3, VectorOptions> returnMove;
    public TweenerCore<Quaternion, Quaternion, NoOptions> returnRotation;
    [SerializeField] private LayerMask interactionLayer;

    private void Awake() {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        canvas = GetComponentInParent<Canvas>(); // canvas нужен для правильного перемещения
    }

    public void OnBeginDrag(PointerEventData eventData) {
        if (!inProgress && !inAddingProgress) {
            originalPosition = rectTransform.position;
            originalRotation = transform.rotation;
        }

        returnMove.Kill();
        returnRotation.Kill();
        inProgress = true;
        var newRotation = transform.rotation;
        newRotation.z = 0f;
        transform.DOLocalRotateQuaternion(newRotation, 0.1f);
        // canvasGroup.alpha = 0.6f; // сделаем полупрозрачным
        // canvasGroup.blocksRaycasts = false; // чтобы не мешал Raycast'ам во время перетаскивания
    }

    public void OnDrag(PointerEventData eventData) {
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData) {
        DetectDropCard();
    }

    public void DetectDropCard() {
        var ray = G.Instance.mainCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out var hit, Mathf.Infinity, interactionLayer)) {
            if (hit.transform.TryGetComponent(out BuildingCardDropArea buildingCardDropArea)) {
                buildingCardDropArea.OnDropCard(GetComponent<CardController>());
                return;
            }
        }
        
        returnMove = transform.DOMove(originalPosition, 0.5f).OnKill(() => { inProgress = false; })
            .OnComplete(() => { inProgress = false; });
        returnRotation = transform.DOLocalRotateQuaternion(originalRotation, 0.5f);
    }
}
}
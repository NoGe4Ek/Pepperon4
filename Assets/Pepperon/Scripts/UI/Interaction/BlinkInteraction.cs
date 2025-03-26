using System.Collections;
using Pepperon.Scripts.Controllers;
using Pepperon.Scripts.Managers;
using Pepperon.Scripts.Utils;
using UnityEngine;

namespace Pepperon.Scripts.UI.Interaction {
public class BlinkInteraction : MonoBehaviour {
    private MeshRenderer meshRenderer;
    private Color originalColor;
    [SerializeField] private Color blinkColor;
    [SerializeField] private Color hoverColor;
    [SerializeField] private float blinkSpeed = 1f;

    [SerializeField] private LayerMask interactionLayer;

    private bool isEnabled;
    private bool isHovered;
    private Camera currentCamera;
    private Task blinkCoroutine;

    private void Start() {
        currentCamera = Camera.main;
        meshRenderer = GetComponentInChildren<MeshRenderer>();
        originalColor = meshRenderer.material.color;
    }

    private void Update() {
        CheckHoverAndClick();
    }

    public void Enable() {
        isEnabled = true;
        StartBlinking();
    }

    public void Disable() {
        isEnabled = false;
        StopBlinking();
    }

    private void StartBlinking() {
        if (blinkCoroutine is not { Running: true })
            blinkCoroutine = new Task(BlinkRoutine());
    }

    private void StopBlinking() {
        isHovered = false;
        blinkCoroutine.Stop();
        meshRenderer.material.color = originalColor;
    }

    private IEnumerator BlinkRoutine() {
        while (!isHovered) {
            var t = Mathf.PingPong(Time.time * blinkSpeed, 1f);
            meshRenderer.material.color = Color.Lerp(originalColor, blinkColor, t);
            yield return null;
        }
    }

    private void CheckHoverAndClick() {
        if (!isEnabled) return;
        var ray = currentCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out var hit, Mathf.Infinity, interactionLayer)) {
            Debug.Log(" Mouse down: " + Input.GetMouseButtonDown(0));
            var isThisObject = hit.collider.gameObject == gameObject;

            switch (isThisObject) {
                case true when !isHovered:
                    SetHovered(true);
                    break;
                case false when isHovered: {
                    SetHovered(false);
                    StartBlinking();
                    break;
                }
            }

            if (isThisObject && Input.GetMouseButtonDown(0))
                OnRegionClicked();
        } else if (isHovered) {
            SetHovered(false);
            StartBlinking();
        }
    }

    private void SetHovered(bool isHovered) {
        this.isHovered = isHovered;
        if (meshRenderer != null) {
            meshRenderer.material.color = isHovered ? hoverColor : originalColor;
        }
    }

    private void OnRegionClicked() {
        var barrackIndex = PlayerController.localPlayer.barracks.IndexOf(gameObject);
        InteractionManager.Instance.OnBarrackChosen(barrackIndex);
    }
}
}
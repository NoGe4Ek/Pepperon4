using Pepperon.Scripts.Controllers;
using Pepperon.Scripts.DI;
using Pepperon.Scripts.Entities.Controllers;
using UnityEngine;
using UnityEngine.UI;

namespace Pepperon.Scripts.Entities.Components {
public class MinimapComponent : MonoBehaviour {
    private GameObject minimapIconPrefab;
    private Image minimapIcon;
    private RectTransform minimapIconRect;
    private const int UpdateFrameInterval = 50;

    private void Start() {
        minimapIconPrefab = GetComponent<EntityController>().entity.minimapIconPrefab;
        minimapIcon = Instantiate(minimapIconPrefab, G.Instance.minimapCanvas.transform).GetComponent<Image>();
        minimapIconRect = minimapIcon.GetComponent<RectTransform>();
        
        minimapIcon.color = GetComponent<EntityController>().playerType == PlayerController.localPlayer.playerId
            ? Color.green
            : Color.red;
    }

    private void Update() {
        if (Time.frameCount % UpdateFrameInterval != 0) return;
        
        Vector3 worldPos = transform.position;
        Vector3 screenPos = G.Instance.minimapCamera.WorldToViewportPoint(worldPos);

        // Нормализуем и масштабируем под размер Canvas
        float canvasWidth = G.Instance.minimapCanvas.GetComponent<RectTransform>().rect.width;
        float canvasHeight = G.Instance.minimapCanvas.GetComponent<RectTransform>().rect.height;

        var minimapPos = new Vector2(
            (screenPos.x * canvasWidth) - (canvasWidth * 0.5f),
            (screenPos.y * canvasHeight) - (canvasHeight * 0.5f)
        );
        minimapIconRect.anchoredPosition = minimapPos;
    }

    private void OnDestroy() {
        Destroy(minimapIcon.gameObject);
    }
}
}
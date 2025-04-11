using System.Collections;
using Pepperon.Scripts.DI;
using Pepperon.Scripts.Utils;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Pepperon.Scripts.Managers {
public class MinimapManager : MonoBehaviour {
    public RectTransform viewportIndicator; // UI Rectangle (Image) on minimap
    public Transform ground;
    public GameObject circlePrefab;
    private Camera mainCamera;
    private Camera minimapCamera;
    private RectTransform minimapRect;
    public RectTransform minimap;
    Vector2[] canvasCorners = new Vector2[4];

    private GameObject[] cornerMarkers = new GameObject[4];
    private Vector2 indicatorSize;
    private Task cameraMovementCoroutine;
    public Vector3 lastCameraTarget;
    public bool isManualMovement = true;
    public bool autoMovementLock = false;

    private void Start() {
        mainCamera = G.Instance.mainCamera;
        minimapCamera = G.Instance.minimapCamera;
        minimapRect = G.Instance.minimapCanvas.GetComponent<RectTransform>();
        lastCameraTarget = G.Instance.mainCamera.transform.position;
        // lastCameraTarget = PlayerController.localPlayer.mainBuilding.transform.position;

        for (int i = 0; i < 4; i++) {
            // Создаем маркер и делаем его дочерним к канвасу
            cornerMarkers[i] = Instantiate(circlePrefab, G.Instance.minimapCanvas.transform);
        }

        minimap.gameObject.AddComponent<GraphicRaycaster>();
        EventTrigger trigger = minimap.gameObject.AddComponent<EventTrigger>();

        var pointerDown = new EventTrigger.Entry { eventID = EventTriggerType.PointerDown };
        pointerDown.callback.AddListener((data) => OnMinimapPointerDown((PointerEventData)data));
        trigger.triggers.Add(pointerDown);

        var drag = new EventTrigger.Entry { eventID = EventTriggerType.Drag };
        drag.callback.AddListener((data) => OnMinimapDrag((PointerEventData)data));
        trigger.triggers.Add(drag);
    }

    private void OnMinimapPointerDown(PointerEventData eventData) {
        MoveCameraToMouse(eventData.position);
    }

    private void OnMinimapDrag(PointerEventData eventData) {
        MoveCameraToMouse(eventData.position);
    }

    private void Update() {
        // Для ортографической камеры фрустум - это прямоугольник
        float orthoHeight = mainCamera.orthographicSize;
        float orthoWidth = orthoHeight * mainCamera.aspect;

        // Углы фрустума в локальных координатах камеры
        Vector3[] frustumCorners = new Vector3[4] {
            new Vector3(-orthoWidth, -orthoHeight,
                new Plane(Vector3.up, ground.position).GetDistanceToPoint(mainCamera.transform.position) +
                orthoHeight), // bottom-left
            new Vector3(orthoWidth, -orthoHeight,
                new Plane(Vector3.up, ground.position).GetDistanceToPoint(mainCamera.transform.position) +
                orthoHeight), // bottom-right
            new Vector3(-orthoWidth, orthoHeight,
                new Plane(Vector3.up, ground.position).GetDistanceToPoint(mainCamera.transform.position) +
                orthoHeight), // top-left
            new Vector3(orthoWidth, orthoHeight,
                new Plane(Vector3.up, ground.position).GetDistanceToPoint(mainCamera.transform.position) +
                orthoHeight) // top-right
        };

        // Преобразуем в мировые координаты
        Vector3[] worldCorners = new Vector3[4];
        for (int i = 0; i < 4; i++) {
            worldCorners[i] = mainCamera.transform.TransformPoint(frustumCorners[i]);
        }

        DrawDebugLines(worldCorners, Color.red);

        RectTransform canvasRect = G.Instance.minimapCanvas.GetComponent<RectTransform>();
        float canvasWidth = canvasRect.rect.width;
        float canvasHeight = canvasRect.rect.height;

        // Массив для хранения позиций на канвасе
        canvasCorners = new Vector2[4];

        for (int i = 0; i < 4; i++) {
            // 1. Проецируем мировые координаты в Viewport Space камеры мини-карты
            Vector3 viewportPos = minimapCamera.WorldToViewportPoint(worldCorners[i]);

            // 3. Преобразуем в координаты канваса (с центром в середине)
            canvasCorners[i] = new Vector2(
                (viewportPos.x * canvasWidth) - (canvasWidth * 0.5f),
                (viewportPos.y * canvasHeight) - (canvasHeight * 0.5f)
            );
        }

        for (int i = 0; i < 4; i++) {
            // Устанавливаем позицию маркера
            RectTransform markerRect = cornerMarkers[i].GetComponent<RectTransform>();
            markerRect.anchoredPosition = canvasCorners[i];
        }

// Определяем границы
        float minX = Mathf.Min(canvasCorners[0].x, canvasCorners[1].x, canvasCorners[2].x, canvasCorners[3].x);
        float maxX = Mathf.Max(canvasCorners[0].x, canvasCorners[1].x, canvasCorners[2].x, canvasCorners[3].x);
        float minY = Mathf.Min(canvasCorners[0].y, canvasCorners[1].y, canvasCorners[2].y, canvasCorners[3].y);
        float maxY = Mathf.Max(canvasCorners[0].y, canvasCorners[1].y, canvasCorners[2].y, canvasCorners[3].y);

// Центр и размер
        Vector2 center = new Vector2((minX + maxX) * 0.5f, (minY + maxY) * 0.5f);
        Vector2 size = new Vector2(maxX - minX, maxY - minY);
        indicatorSize = size;

// Устанавливаем размер и позицию индикатора
        viewportIndicator.anchoredPosition = center;
        viewportIndicator.sizeDelta = size;

        if (Vector3.Distance(mainCamera.transform.position, lastCameraTarget) < 0.1f) {
            if (autoMovementLock)
                autoMovementLock = false;
            return;
        }

        if (isManualMovement)
            return;

        // Debug.Log("Lerp");
        // Debug.Log("Distance: " + Vector3.Distance(mainCamera.transform.position, lastCameraTarget));
        mainCamera.transform.position = Vector3.Lerp(
            mainCamera.transform.position,
            lastCameraTarget,
            Time.deltaTime * 10f
        );
    }

    void DrawDebugLines(Vector3[] corners, Color color) {
        // Отрисовка линий между углами (в мировых координатах)
        Debug.DrawLine(corners[0], corners[1], color); // Нижняя грань (bottom-left → bottom-right)
        Debug.DrawLine(corners[1], corners[3], color); // Правая грань (bottom-right → top-right)
        Debug.DrawLine(corners[3], corners[2], color); // Верхняя грань (top-right → top-left)
        Debug.DrawLine(corners[2], corners[0], color); // Левая грань (top-left → bottom-left)

        // Дополнительно: рисуем диагонали для наглядности
        Debug.DrawLine(corners[0], corners[3], color); // Диагональ 1 (bottom-left → top-right)
        Debug.DrawLine(corners[1], corners[2], color); // Диагональ 2 (bottom-right → top-left)
    }


    private Vector3 cameraVelocity = Vector3.zero; // Инициализация

    private void MoveCameraToMouse(Vector2 mousePosition) {
        isManualMovement = false;
        autoMovementLock = true;

        float widthMultiplier = minimapCamera.scaledPixelWidth / minimap.rect.width;
        float heightMultiplier = minimapCamera.scaledPixelHeight / minimap.rect.height;
        Vector2 convertedMousePosition = new(
            (minimap.rect.width - (Screen.width - mousePosition.x)) * widthMultiplier,
            (mousePosition.y - 30f - indicatorSize.y / 2) * heightMultiplier
        );

        Ray cameraRay = minimapCamera.ScreenPointToRay(convertedMousePosition);
        // Рисуем луч в сцене (видно только в редакторе в режиме Play)
        Debug.DrawRay(cameraRay.origin, cameraRay.direction * 100f, Color.red, 1f);

        if (Physics.Raycast(cameraRay, out RaycastHit hit, float.MaxValue)) {
            var newCameraPosition = hit.point;
            newCameraPosition.y = mainCamera.transform.position.y;
            newCameraPosition.z -= 20f;
            newCameraPosition.x += 8f;

            lastCameraTarget = newCameraPosition;

            // cameraMovementCoroutine = new Task(SmoothCameraMove(newCameraPosition));

            // if (Vector3.Distance(mainCamera.transform.position, lastCameraTarget) > 0.1f)
            // {
            //     mainCamera.transform.position = Vector3.SmoothDamp(
            //         mainCamera.transform.position,
            //         lastCameraTarget,
            //         ref cameraVelocity,
            //         0.1f, // Время сглаживания
            //         500f    // Макс. скорость
            //     );
            // }

            Debug.DrawLine(hit.point, hit.point + Vector3.up * 10f, Color.green, 1f);
        }
    }

    private IEnumerator SmoothCameraMove(Vector3 targetPosition, float duration = 0.1f) {
        Vector3 startPosition = mainCamera.transform.position;
        float elapsedTime = 0f;

        // targetPosition.y = startPosition.y;
        // targetPosition.z -= 28f;

        while (elapsedTime < duration) {
            if (mainCamera.transform.position == lastCameraTarget) continue;

            float t = 1f - Mathf.Pow(1f - (elapsedTime / duration), 2f);
            mainCamera.transform.position = Vector3.Lerp(startPosition, targetPosition, t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        mainCamera.transform.position = targetPosition;
    }

    public static MinimapManager Instance { get; private set; }

    private void Awake() {
        Instance = this;
    }
}
}
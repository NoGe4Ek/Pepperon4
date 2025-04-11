using System.Collections;
using Pepperon.Scripts.DI;
using UnityEngine;

namespace Pepperon.Scripts.UI {
public static class UIG {
    public static IEnumerator SmoothMove(Component target, Vector3 targetPosition, float duration = 0.1f) {
        Vector3 startPosition = target.transform.position;
        float elapsedTime = 0f;

        targetPosition.y = startPosition.y;
        targetPosition.z -= 28f;

        while (elapsedTime < duration) {
            float t = 1f - Mathf.Pow(1f - (elapsedTime / duration), 2f);
            target.transform.position = Vector3.Lerp(startPosition, targetPosition, t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        target.transform.position = targetPosition;
    }
}
}
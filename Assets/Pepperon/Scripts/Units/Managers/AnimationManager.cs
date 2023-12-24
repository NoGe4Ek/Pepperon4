using Pepperon.Scripts.AI.Units.Controllers;
using Pepperon.Scripts.AI.Units.ScriptableObjects;
using UnityEngine;

public class AnimationManager : MonoBehaviour {
    private UnitSO unit;

    // Should be public with setter
    public Vector3 PointerInput { get; set; }

    private void Awake() {
        unit = GetComponent<UnitController>().unit;
    }

    private void Update() {
        RotateToPointer(PointerInput);
    }

    private void RotateToPointer(Vector3 lookDirection) {
        if (lookDirection.magnitude > 0.001f) {
            lookDirection.y = 0;
            Quaternion targetRotation = Quaternion.LookRotation(lookDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, unit.rotationSpeed * Time.deltaTime);
        }
    }
}
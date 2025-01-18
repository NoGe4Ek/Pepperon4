using Pepperon.Scripts.Entities.Controllers;
using Pepperon.Scripts.Units.Data;
using Pepperon.Scripts.Utils;
using UnityEngine;

namespace Pepperon.Scripts.Entities.Detectors {
public class ObstacleDetector : MonoBehaviour {
    [SerializeField] private LayerMask obstacleLayerMask; // rename

    private MovementData movementData;
    private UnitController unitController;

    private void Awake() {
        movementData = GetComponentInParent<MovementData>();
        unitController = GetComponentInParent<UnitController>();
    }

    public void OnObstacleTriggerEnter(OnTriggerDelegation delegation) {
        if (delegation.Caller.name != "ObstacleRange") return;
        delegation.Other.TryGetComponent(out UnitController otherUnit);
        if ((obstacleLayerMask.value & (1 << delegation.Other.gameObject.layer)) == 0 &&
            (!otherUnit || otherUnit.playerType != unitController.playerType)) return;

        // Debug.Log("Trigger obstacle Enter");
        // if (otherUnit != null) {
        //     movementData.units.Add(otherUnit);
        // }
        // else {
        movementData.obstacles.Add(delegation.Other);
        // }
    }

    public void OnObstacleTriggerExit(OnTriggerDelegation delegation) {
        if (delegation.Caller.name != "ObstacleRange") return;
        delegation.Other.TryGetComponent(out UnitController otherUnit);
        if ((obstacleLayerMask.value & (1 << delegation.Other.gameObject.layer)) == 0 &&
            (!otherUnit || otherUnit.playerType != unitController.playerType)) return;

        // Debug.Log("Trigger obstacle Exit");
        // if (otherUnit != null) {
        //     movementData.units.Remove(otherUnit);
        // }
        // else {
        movementData.obstacles.Remove(delegation.Other);
        // }
    }
}
}
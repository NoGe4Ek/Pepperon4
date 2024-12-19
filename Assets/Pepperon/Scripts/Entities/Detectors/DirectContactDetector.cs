using Pepperon.Scripts.AI;
using Pepperon.Scripts.Entities.Controllers;
using Pepperon.Scripts.Units.Data;
using Pepperon.Scripts.Utils;
using UnityEngine;

namespace Pepperon.Scripts.Units.Detectors {
public class DirectContactDetector : MonoBehaviour {
    private MovementData movementData;

    private void Awake() {
        movementData = GetComponentInParent<MovementData>();
    }

    public void OnDirectContactTriggerEnter(OnTriggerDelegation delegation) {
        if (delegation.Caller.name != "DirectContactRange") return;
        if (!delegation.Other.TryGetComponent(out UnitController otherUnit)) return;
        if (!otherUnit.stateMachineComponent.IsRunning()) return;
        
        movementData.directContact.Add(delegation.Other);
    }

    public void OnDirectContactTriggerExit(OnTriggerDelegation delegation) {
        if (delegation.Caller.name != "DirectContactRange") return;
        if (!delegation.Other.TryGetComponent(out UnitController otherUnit)) return;
        
        movementData.directContact.Remove(delegation.Other);
    }
}
}
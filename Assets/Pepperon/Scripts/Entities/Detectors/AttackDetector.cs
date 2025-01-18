using Pepperon.Scripts.Entities.Controllers;
using Pepperon.Scripts.Units.Data;
using Pepperon.Scripts.Utils;
using UnityEngine;

namespace Pepperon.Scripts.Entities.Detectors {
public class AttackDetector : MonoBehaviour {
    [SerializeField] private LayerMask playerLayerMask; // rename
    
    private AttackingData attackingData;
    private UnitController unitController;

    private void Awake() {
        attackingData = GetComponentInParent<AttackingData>();
        unitController = GetComponentInParent<UnitController>();
    }
    
    public void OnAttackTriggerEnter(OnTriggerDelegation delegation) {
        if (delegation.Caller.name != "AttackRange") return;
        // check mask?
        if (!delegation.Other.TryGetComponent(out EntityController otherUnit)) return;
        if (otherUnit.playerType == unitController.playerType) return;
        Collider enemyCollider = otherUnit.GetComponentInParent<CapsuleCollider>();
        // Debug.Log("Trigger attack Enter");
        
        attackingData.targets.Add(enemyCollider.transform);
    }
    
    public void OnAttackTriggerExit(OnTriggerDelegation delegation) {
        if (delegation.Caller.name != "AttackRange") return;
        if (!delegation.Other.TryGetComponent(out EntityController otherUnit)) return;
        if (otherUnit.playerType == unitController.playerType) return;
        Collider enemyCollider = otherUnit.GetComponentInParent<CapsuleCollider>();
        // Debug.Log("Trigger attack Exit");

        var enemyTransform = enemyCollider.transform;
        if (enemyTransform == attackingData.currentTarget) {
            attackingData.currentTarget = null;
        }
        attackingData.targets.Remove(enemyTransform);
    }
}
}
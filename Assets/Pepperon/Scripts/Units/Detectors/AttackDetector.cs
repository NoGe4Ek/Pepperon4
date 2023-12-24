using Pepperon.Scripts.AI;
using Pepperon.Scripts.AI.Units.Controllers;
using Pepperon.Scripts.AI.Units.ScriptableObjects;
using Pepperon.Scripts.Utils;
using UnityEngine;

namespace Pepperon.Scripts.Units.Detectors {
public class AttackDetector : MonoBehaviour {
    [SerializeField] private LayerMask playerLayerMask; // rename
    
    private AttackingData attackingData;

    private void Awake() {
        attackingData = GetComponentInParent<AttackingData>();
    }
    
    public void OnAttackTriggerEnter(OnTriggerDelegation delegation) {
        if (delegation.Caller.name != "AttackRange") return;
        // check mask?
        if (!delegation.Other.TryGetComponent(out UnitController otherUnit)) return;
        Collider enemyCollider = otherUnit.GetComponentInParent<CapsuleCollider>();
        Debug.Log("Trigger attack Enter");
        
        attackingData.targets.Add(enemyCollider.transform);
    }
    
    public void OnAttackTriggerExit(OnTriggerDelegation delegation) {
        if (delegation.Caller.name != "AttackRange") return;
        if (!delegation.Other.TryGetComponent(out UnitController otherUnit)) return;
        Collider enemyCollider = otherUnit.GetComponentInParent<CapsuleCollider>();
        Debug.Log("Trigger attack Exit");
        
        attackingData.targets.Remove(enemyCollider.transform);
    }
}
}
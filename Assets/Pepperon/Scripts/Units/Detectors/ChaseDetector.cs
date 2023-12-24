using Pepperon.Scripts.AI;
using Pepperon.Scripts.AI.Units.Controllers;
using Pepperon.Scripts.AI.Units.ScriptableObjects;
using Pepperon.Scripts.Utils;
using UnityEngine;

public class ChaseDetector : MonoBehaviour {
    [SerializeField] private LayerMask obstaclesLayerMask, playerLayerMask; // rename
    
    private MovementData movementData;
    private UnitSO unit;

    private void Awake() {
        movementData = GetComponentInParent<MovementData>();
        unit = GetComponentInParent<UnitController>().unit;
    }
    
    public void OnChaseTriggerEnter(OnTriggerDelegation delegation) {
        if (delegation.Caller.name != "ChaseRange") return;
        // check mask?
        if (!delegation.Other.TryGetComponent(out UnitController otherUnit)) return;
        Collider enemyCollider = otherUnit.GetComponentInParent<CapsuleCollider>();
        Debug.Log("Trigger chase Enter");

        //Check if you see the player
        Vector3 direction = (enemyCollider.transform.position - transform.position).normalized;

        //Make sure that the collider we see is on the "Player" layer
        if (!Physics.Raycast(transform.position, direction, out var hit, unit.chaseRange,
                obstaclesLayerMask)) return;
        if ((playerLayerMask & (1 << hit.collider.gameObject.layer)) == 0) return;
        
        movementData.targets.Add(enemyCollider.transform);
    }
    
    public void OnChaseTriggerExit(OnTriggerDelegation delegation) {
        if (delegation.Caller.name != "ChaseRange") return;
        if (!delegation.Other.TryGetComponent(out UnitController otherUnit)) return;
        Collider enemyCollider = otherUnit.GetComponentInParent<CapsuleCollider>();
        Debug.Log("Trigger chase Exit");
        movementData.targets.Remove(enemyCollider.transform);
    }
}
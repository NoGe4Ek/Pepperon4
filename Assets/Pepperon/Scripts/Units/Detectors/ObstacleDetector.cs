using Pepperon.Scripts.AI;
using Pepperon.Scripts.AI.Units.Controllers;
using Pepperon.Scripts.AI.Units.ScriptableObjects;
using Pepperon.Scripts.Utils;
using UnityEngine;

public class ObstacleDetector : MonoBehaviour {
    [SerializeField] private LayerMask obstacleLayerMask;

    private MovementData movementData;

    private void Awake() {
        movementData = GetComponentInParent<MovementData>();
    }

    public void OnObstacleTriggerEnter(OnTriggerDelegation delegation) {
        if (delegation.Caller.name != "ObstacleRange") return;
        if ((obstacleLayerMask.value & (1 << delegation.Other.gameObject.layer)) == 0) return;
        Debug.Log("Trigger obstacle Enter");
        movementData.obstacles.Add(delegation.Other);
    }
    
    public void OnObstacleTriggerExit(OnTriggerDelegation delegation) {
        if (delegation.Caller.name != "ObstacleRange") return;
        if ((obstacleLayerMask.value & (1 << delegation.Other.gameObject.layer)) == 0) return;
        Debug.Log("Trigger obstacle Exit");
        movementData.obstacles.Remove(delegation.Other);
    }
}
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Pepperon.Scripts.AI;
using Pepperon.Scripts.AI.Units.Controllers;
using Pepperon.Scripts.AI.Units.ScriptableObjects;
using UnityEngine;

public class SeekBehaviour : SteeringBehaviour {
    [SerializeField] private bool showGizmo = true;

    private float[] interestsTemp;
    private Vector3 cached;
    
    
    private MovementData movementData;
    private UnitSO unit;

    private void Awake() {
        movementData = GetComponentInParent<MovementData>();
        unit = GetComponentInParent<UnitController>().unit;
    }

    public override (float[] danger, float[] interest) GetSteering(float[] danger, float[] interest) {
        cached = movementData.currentTargetPosition;
        
        //if we don't have a target stop seeking
        //else set a new target
        // if (reachedLastTarget) {
        //     if (movementData.targets == null || movementData.targets.Count <= 0) {
        //         reachedLastTarget = false;
        //         movementData.CurrentTarget = movementData.GetNextPoint();
        //     }
        //     else {
        //         reachedLastTarget = false;
        //         movementData.CurrentTarget = movementData.targets.OrderBy
        //             (target => Vector3.Distance(target.position, transform.position)).FirstOrDefault();
        //     }
        // }

        //First check if we have reached the target
        if (movementData.MovementState != MovementData.MovementStateEnum.NotMoving &&
            Vector3.Distance(transform.position, movementData.currentTargetPosition) < unit.targetReachedThreshold) {
            //reachedLastTarget = true;
            movementData.CurrentTarget = null;
            movementData.currentTargetPosition = Vector3.zero;
            return (danger, interest);
        }

        //If we havent yet reached the target do the main logic of finding the interest directions
        Vector3 directionToTarget = (movementData.currentTargetPosition - transform.position);
        for (int i = 0; i < interest.Length; i++) {
            float result = Vector3.Dot(directionToTarget.normalized, Directions.eightDirections[i]);

            //accept only directions at the less than 90 degrees to the target direction
            if (result > 0) {
                float valueToPutIn = result;
                if (valueToPutIn > interest[i]) {
                    interest[i] = valueToPutIn;
                }
            }
        }

        interestsTemp = interest;
        return (danger, interest);
    }

    private void OnDrawGizmos() {
        if (showGizmo == false)
            return;
        Gizmos.DrawSphere(cached, 0.2f);

        if (Application.isPlaying && interestsTemp != null) {
            if (interestsTemp != null) {
                Gizmos.color = Color.green;
                for (int i = 0; i < interestsTemp.Length; i++) {
                    Gizmos.DrawRay(transform.position, Directions.eightDirections[i] * interestsTemp[i] * 2);
                }
            }
        }
    }
}
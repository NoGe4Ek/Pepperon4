using System;
using Pepperon.Scripts.Entities.MovementSystem.Behaviours;
using Pepperon.Scripts.Entities.Systems.LoreSystem.Base.Infos;
using Pepperon.Scripts.Units.Components;
using Pepperon.Scripts.Units.Data;
using UnityEngine;

public class SeekBehaviour : BaseSteeringBehaviour {
    [SerializeField] private bool showCachedTargetGizmo;
    private Vector3 cached;

    private MovementData movementData;
    private MovementInfo movementInfo;

    private void Awake() {
        movementData = GetComponentInParent<MovementData>();
    }

    private void Start() {
        movementInfo = GetComponentInParent<MovementComponent>().movementInfo;
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
            Vector3.Distance(transform.position, movementData.currentTargetPosition) < 0.1) {
            movementData.MovementState = MovementData.MovementStateEnum.PointReached;
            movementData.currentTarget = null;
            movementData.currentTargetPosition = Vector3.zero;
            // reachedLastTarget = true;
            // movementData.currentTarget = null;
            // movementData.currentTargetPosition = Vector3.zero;
            return (danger, interest);
        }

        //If we havent yet reached the target do the main logic of finding the interest directions
        Vector3 directionToTarget = (movementData.currentTargetPosition - transform.position);
        for (int i = 0; i < interest.Length; i++) {
            float result = Vector3.Dot(directionToTarget.normalized, Directions.directions[i]);

            //accept only directions at the less than 90 degrees to the target direction
            if (result > 0) {
                float valueToPutIn = result;
                if (valueToPutIn > interest[i]) {
                    interest[i] = valueToPutIn;
                }
            }
        }

        return (danger, interest);
    }

    private void OnDrawGizmos() {
        if (showCachedTargetGizmo == false) return;
        Gizmos.DrawSphere(cached, 0.2f);
    }
}
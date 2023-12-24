using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Pepperon.Scripts.AI {
public class MovementData : MonoBehaviour {
    [SerializeField] public List<Transform> points;

    public enum MovementStateEnum {
        NotMoving,
        MovingToPoint,
        MovingToTarget,
        MovingToCached
    }

    public MovementStateEnum MovementState { private set; get; } = MovementStateEnum.NotMoving;

    public List<Transform> targets;
    public List<Collider> obstacles;
    public Transform CurrentTarget { get; set; }
    public Vector3 currentTargetPosition;

    private void Update() {
        if (CurrentTarget) {
            if (MovementState == MovementStateEnum.MovingToTarget && !targets.Contains(CurrentTarget)) {
                CurrentTarget = null;
                MovementState = MovementStateEnum.MovingToCached;
            }
            else if (MovementState == MovementStateEnum.MovingToPoint && targets is { Count: > 0 }) {
                CurrentTarget = targets.OrderBy(target =>
                    Vector3.Distance(target.position, transform.position)
                ).FirstOrDefault();
                MovementState = MovementStateEnum.MovingToTarget;
            }
            else {
                currentTargetPosition = CurrentTarget.position;
            }
        }
        else {
            if (targets is { Count: > 0 }) {
                CurrentTarget = targets.OrderBy(target =>
                    Vector3.Distance(target.position, transform.position)
                ).FirstOrDefault();
                MovementState = MovementStateEnum.MovingToTarget;
            }
            else if (currentTargetPosition != Vector3.zero) {
                MovementState = MovementStateEnum.MovingToCached;
            }
            else {
                CurrentTarget = GetNextPoint();
                MovementState = MovementStateEnum.MovingToPoint;
            }
        }
    }

    private Transform GetNextPoint() => points[Random.Range(0, points.Count)];
}
}
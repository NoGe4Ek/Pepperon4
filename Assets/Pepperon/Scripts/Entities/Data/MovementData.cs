using System.Collections.Generic;
using System.Linq;
using Mirror;
using UnityEngine;

// todo on release change lists with hash set
namespace Pepperon.Scripts.Units.Data {
public class MovementData : NetworkBehaviour {
    [SerializeField] public List<Transform> points = new();

    public enum MovementStateEnum {
        NotMoving,
        MovingToPoint,
        MovingToTarget,
        PointReached
    }

    public MovementStateEnum MovementState { set; get; } = MovementStateEnum.NotMoving;
    [SerializeField] public List<Transform> targets = new();
    [SerializeField] public List<Collider> obstacles;
    [SerializeField] public List<Collider> directContact;

    [SerializeField] public Transform currentTarget;
    [SerializeField] public Vector3 currentTargetPosition;
    public Transform GetNextPoint() => points.First();
}
}
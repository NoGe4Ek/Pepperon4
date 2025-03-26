using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Mirror;
using Pepperon.Scripts.Entities.Controllers;
using Pepperon.Scripts.Entities.MovementSystem.Behaviours;
using Pepperon.Scripts.Managers;
using Pepperon.Scripts.Systems.LoreSystem.Base.Infos;
using Pepperon.Scripts.Units.Data;
using UnityEngine;

// todo limit to send velocity updates or make interpolation lerp on client side
// Vector3.Lerp(previousVelocity, rb.velocity, Time.fixedDeltaTime * syncRate);
namespace Pepperon.Scripts.Units.Components {
public class MovementComponent : NetworkBehaviour {
    public bool isActive = true;

    public void Disable() {
        isActive = false;
        // rb.velocity = Vector3.zero;
        // RpcSyncMovement(Vector3.zero);
        StopAllCoroutines();
    }

    public MovementInfo movementInfo;
    public MovementInfoProgress movementInfoProgress;
    [SerializeField] public MovementData movementData;
    [SerializeField] public List<BaseSteeringBehaviour> behaviours;
    [SerializeField] public ContextSolver movementDirectionSolver;
    public bool isChasing = true;

    [SerializeField] private Transform obstacleRange;
    [SerializeField] private Transform chaseRange;
    [SerializeField] private Transform directContactRange;

    private AnimationComponent animationComponent;

    [SerializeField] private bool shouldRotateToTarget;

    // [ConditionalDisplay(nameof(shouldRotateToTarget)), 
    [SerializeField] private RotationComponent rotationComponent;

    private Rigidbody rb;
    private Vector3 oldMovementInput;
    private float currentSpeed;

    // Should be public with setter
    public Vector3 MovementDirection { private get; set; }

    private void Awake() {
        animationComponent = GetComponent<AnimationComponent>();
        rb = GetComponent<Rigidbody>();

        obstacleRange.GetComponent<Renderer>().enabled = false;
        chaseRange.GetComponent<Renderer>().enabled = false;
        directContactRange.GetComponent<Renderer>().enabled = false;
    }

    private void Start() {
        movementInfo = GetComponent<EntityController>().entity.info.OfType<MovementInfo>().First();
        movementInfoProgress =
            GetComponent<EntityController>().entityProgress.info.OfType<MovementInfoProgress>().First();
    }

    private void FixedUpdate() {
        if (!isActive) return;
        if (!isServer) return;

        Move();
        // RpcSyncMovement(rb.velocity);
    }

    private void Update() {
        if (!isActive) return;

        obstacleRange.localScale =
            new Vector3(movementInfoProgress.obstacleRange, movementInfoProgress.obstacleRange,
                movementInfoProgress.obstacleRange);
        chaseRange.localScale = new Vector3(movementInfoProgress.chaseRange, movementInfoProgress.chaseRange,
            movementInfoProgress.chaseRange);
        animationComponent.SetFloat(movementInfo.runningAnimationSpeedName, movementInfoProgress.speed);

        if (!isServer) return;
        FindTarget();
        CalculateMovementDirection();
    }

    private void CalculateMovementDirection() {
        // Enemy AI movement based on Target availability
        if (movementData.MovementState != MovementData.MovementStateEnum.NotMoving) {
            // Looking at the Target
            if (shouldRotateToTarget)
                rotationComponent.RotationDirection = MovementDirection;
            StartCoroutine(ChaseAndAttack());
        }
    }

    private void FindTarget() {
        if (movementData.currentTarget) {
            if (movementData.MovementState == MovementData.MovementStateEnum.MovingToTarget &&
                !movementData.targets.Contains(movementData.currentTarget)) {
                movementData.currentTargetPosition = Vector3.zero;

                movementData.currentTarget = null;
            }
            else if (movementData.MovementState == MovementData.MovementStateEnum.MovingToPoint &&
                     movementData.targets.Count > 0) {
                movementData.currentTarget = movementData.targets
                    .OrderBy(target =>
                        Vector3.Distance(target.position, transform.position)
                    ).FirstOrDefault();
                movementData.MovementState = MovementData.MovementStateEnum.MovingToTarget;
            }
            else {
                movementData.currentTargetPosition = movementData.currentTarget.position;
            }
        }
        else {
            if (movementData.targets is { Count: > 0 }) {
                movementData.currentTarget = movementData.targets
                    .OrderBy(target =>
                        Vector3.Distance(target.position, transform.position)
                    ).FirstOrDefault();
                movementData.MovementState = MovementData.MovementStateEnum.MovingToTarget;
            }
            else {
                if (movementData.MovementState == MovementData.MovementStateEnum.PointReached) {
                    // todo fix cycle points
                    var reachedPoint = movementData.points.First();
                    movementData.points.RemoveAt(0);
                    movementData.points.Add(reachedPoint);
                }

                movementData.currentTarget = movementData.GetNextPoint();
                movementData.MovementState = MovementData.MovementStateEnum.MovingToPoint;
            }
        }
    }

    private void Move() {
        if (MovementDirection.magnitude > 0 && currentSpeed >= 0) {
            oldMovementInput = MovementDirection;
            currentSpeed += movementInfoProgress.acceleration * movementInfoProgress.speed * Time.deltaTime;
        }
        else {
            currentSpeed -= movementInfoProgress.deacceleration * movementInfoProgress.speed * Time.deltaTime;
        }

        currentSpeed = Mathf.Clamp(currentSpeed, 0, movementInfoProgress.speed);
        var newPosition = rb.position + oldMovementInput * (currentSpeed * Time.deltaTime);
        transform.position = newPosition;

        // if (Input.GetKey(KeyCode.Space)) {
        //     Debug.Log("MOVE + " + transform.name);
        //
        //     Vector3 newPosition = transform.position + new Vector3(1, 0, 1) * Time.fixedDeltaTime * 3;
        //     // Vector3 newPosition = rb.position + oldMovementInput * (currentSpeed * Time.deltaTime);
        //     // transform.position = newPosition;
        //     rb.MovePosition(newPosition);
        // }
        // rb.velocity = oldMovementInput * currentSpeed;
    }

    // [ClientRpc]
    // private void RpcSyncMovement(Vector3 velocity) {
    //     rb.velocity = velocity;
    // }

    private IEnumerator ChaseAndAttack() {
        //Chase logic
        if (isChasing) {
            MovementDirection = movementDirectionSolver.GetDirectionToMove(behaviours);
            yield return new WaitForSeconds(ProjectSettingsManager.Instance.aiUpdateDelay);
        }
    }
}
}
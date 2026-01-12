using System;
using System.Collections.Generic;
using System.Linq;
using Pepperon.Scripts.AI;
using Pepperon.Scripts.Entities.MovementSystem.Behaviours;
using Pepperon.Scripts.Units.Data;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

public class ContextSolver : MonoBehaviour {
    [SerializeField] private bool showResultGizmo;
    [SerializeField] private bool showInterestsGizmo;
    [SerializeField] private bool showDangersGizmo;
    [SerializeField] private Vector3 resultTemp;
    [SerializeField] private float[] interestsTemp;
    [SerializeField] private float[] dangersTemp;
    [SerializeField] private float[] trueDangersTemp;
    private const float RayLength = 2;

    private int lockIndex = -1;
    [SerializeField] private float[] lockDangers = new float[Directions.DirectionCount];
    private int startDangerLockIndex = -1;

    [SerializeField] private int rightLeft = -1; // right == 1, left == -1

    [SerializeField] private bool isTryTurn = false;
    [SerializeField] private int turnIndex = -1;

    // fix adhesion
    private int holdPositionCounter = 0;
    private int holdPositionTime = 0;
    private int notLockTwiceCounter = 0;
    private int notLockTwiceTime = 30;

    private const float FirstLockThreshold = 0.90f;
    private const float LockThreshold = 0.80f;
    private const float SideVisionLockThreshold = 0.70f;
    private const float FirstUnlockDangersThreshold = 0.68f;
    private const float UnlockInterestThreshold = 0.7f;
    private const float TotalUnlockDangerAngle = 89f;
    private const float TotalUnlockInterestAngle = 46f;

    private MovementData movementData;

    private void Awake() {
        movementData = GetComponentInParent<MovementData>();
    }

    public Vector3 GetDirectionToMove(List<BaseSteeringBehaviour> behaviours) {
        var danger = new float[Directions.DirectionCount];
        var interest = new float[Directions.DirectionCount];
        // Get original interests and dangers
        foreach (BaseSteeringBehaviour behaviour in behaviours) {
            (danger, interest) = behaviour.GetSteering(danger, interest);
        }

        trueDangersTemp = (float[])danger.Clone();

        var maxInterestIndex = GetIndexOfMax(interest);
        var maxDangerIndex = GetIndexOfMax(danger);
        var maxInterestVector = Directions.directions[maxInterestIndex];
        var maxDangerVector = Directions.directions[maxDangerIndex];
        var targetIndex = lockIndex == -1 ? maxInterestIndex : lockIndex;

        // release danger lock if can side move
        if (lockIndex != -1) {
            if (danger[GetPreviousSideIndex(lockIndex, 2)] < FirstUnlockDangersThreshold &&
                danger[GetPreviousSideIndex(lockIndex, 3)] < 0.90f) {
                lockDangers[GetPreviousSideIndex(lockIndex, 1)] = 0;
            }
        }

        // try to squeeze between dangers
        if (movementData.currentTarget && !isTryTurn)
            for (int i = 0; i < Directions.DirectionCount; i++) {
                var turnDanger = danger[i];
                var turnVector = Directions.directions[i];
                float angleInterest1 = Vector3.Angle(turnVector, maxInterestVector);
                float distance = Vector3.Distance(transform.position, movementData.currentTarget.position);
                if (danger[GetPreviousSideIndex(i, 1)] - turnDanger > 0.09
                    && danger[GetNextSideIndex(i, 1)] - turnDanger > 0.09
                    && danger[GetPreviousSideIndex(i, 1)] < 0.90
                    && danger[GetNextSideIndex(i, 1)] < 0.90
                    && angleInterest1 <= 30
                    && distance < 1.5f) {
                    lockIndex = i;
                    lockDangers = new float[Directions.DirectionCount];
                    isTryTurn = true;
                    // Debug.Log("Try turn " + transform.parent.parent.name);
                    turnIndex = i;
                }
            }

        if (isTryTurn && (danger[targetIndex] > 0.95f || danger[turnIndex] > 0.95f)) {
            turnIndex = -1;
            isTryTurn = false;
        }

        // try to hold position if blocked
        if (
            danger[targetIndex] > LockThreshold
            && danger[GetNextSideIndex(maxInterestIndex, Directions.DirectionCount / 2 / 2)] > 0.70
            && danger[GetPreviousSideIndex(maxInterestIndex, Directions.DirectionCount / 2 / 2)] > 0.70
            && danger[GetPreviousSideIndex(maxInterestIndex, Directions.DirectionCount / 2)] > 0.70
        ) {
            return Vector3.zero;
        }

        // if too close side vectors exist
        if (movementData.directContact.Any() && holdPositionCounter == 0 && notLockTwiceCounter == 0) {
            holdPositionTime = Random.Range(0, 21);
            holdPositionCounter += 1;
            lockDangers = new float[Directions.DirectionCount];
        }

        if (holdPositionCounter > 0) {
            if (holdPositionCounter > holdPositionTime) {
                holdPositionCounter = 0;
                notLockTwiceCounter += 1;
            }
            else {
                holdPositionCounter++;
                return Directions.directions[GetNextSideIndex(targetIndex, Directions.DirectionCount / 2)];
            }
        }

        if (notLockTwiceCounter > 0) {
            if (notLockTwiceCounter > notLockTwiceTime) {
                notLockTwiceCounter = 0;
            }
            else {
                notLockTwiceCounter++;
            }
        }

        // upgrade side vision
        bool isSideVisionOn = lockDangers[GetPreviousSideIndex(targetIndex, 1)] > 0.99
                              || lockDangers[GetNextSideIndex(targetIndex, 1)] > 0.99;

        // lock vectors between
        if (!isTryTurn) {
            for (int i = 0; i < Directions.DirectionCount; i++) {
                // lock between (not enough place to go)
                if (lockDangers[i] > 0.99 /* == 1*/ && lockDangers[GetNextSideIndex(i, 2)] > 0.99 /* == 1*/)
                    lockDangers[GetNextSideIndex(i, 1)] = 1f;

                // lock vectors to different side, cause we try to another
                if (lockDangers[i] > 0.99 /* == 1*/ && interest[GetPreviousSideIndex(i, 1)] > 0)
                    lockDangers[GetPreviousSideIndex(i, 1)] = 1f;
                if (lockDangers[i] > 0.99 /* == 1*/ && interest[GetPreviousSideIndex(i, 2)] > 0)
                    lockDangers[GetPreviousSideIndex(i, 2)] = 1f;
                if (lockDangers[i] > 0.99 /* == 1*/ && interest[GetPreviousSideIndex(i, 3)] > 0)
                    lockDangers[GetPreviousSideIndex(i, 3)] = 1f;
                if (lockDangers[i] > 0.99 /* == 1*/ && interest[GetPreviousSideIndex(i, 4)] > 0)
                    lockDangers[GetPreviousSideIndex(i, 4)] = 1f;
            }
        }

        // ============ Dangers change below ==================

        // lock dangers and choose seek force
        int newLockIndex = lockIndex;
        for (int i = 0; i < Directions.DirectionCount; i++) {
            var isVisibilityVector = i == targetIndex
                                     || GetPreviousSideIndex(targetIndex, 1) == i
                                     || GetNextSideIndex(targetIndex, 1) == i;
            var isDangerVector =
                (startDangerLockIndex == -1 && danger[i] > FirstLockThreshold) ||
                startDangerLockIndex != -1 &&
                ((isSideVisionOn && danger[i] > SideVisionLockThreshold) ||
                 (!isSideVisionOn && danger[i] > LockThreshold));
            if (
                // main logic, lock vectors and apply force (LVAF) only if danger visibility vector exist
                isVisibilityVector && isDangerVector
                // if already has locks should continue lock logic
                || lockDangers[i].IsLock()
            ) {
                if (!isTryTurn || lockDangers[i] > 0.99) {
                    danger[i] = 1;
                    lockDangers[i] = 1;
                }

                if (startDangerLockIndex == -1)
                    startDangerLockIndex = maxInterestIndex;
                if (
                    StepCount(targetIndex, GetNextSideIndex(i, 1)) < StepCount(targetIndex, newLockIndex)
                    // not choose the same
                    && StepCount(targetIndex, GetNextSideIndex(i, 1)) != 0
                    // choose any if not set
                    || newLockIndex == -1
                    // not choose blocked one
                    || newLockIndex == i
                )
                    newLockIndex = GetNextSideIndex(i, 1);
            }
        }

        lockIndex = newLockIndex;

        // subtract danger values from interest array
        for (int i = 0; i < Directions.DirectionCount; i++) {
            interest[i] = Mathf.Clamp01(interest[i] - danger[i]);
        }

        // change direction
        if (startDangerLockIndex != -1) {
            var sum = Enumerable.Range(0, 10).Select(i => lockDangers[GetNextSideIndex(startDangerLockIndex, i)]).Sum();

            if (sum > 9) {
                lockIndex = GetPreviousSideIndex(startDangerLockIndex, 1);
                lockDangers = new float[Directions.DirectionCount];
                lockDangers[startDangerLockIndex] = 1f;
                rightLeft *= -1;
            }
        }

        // release interest force if can move side
        // todo UnlockInterestThreshold not have sense
        if (lockIndex != -1) {
            if (danger[GetPreviousSideIndex(lockIndex, 1)] < UnlockInterestThreshold)
                lockIndex = -1;
            else
                interest[lockIndex] = 1f;
        }

        //get the average direction
        Vector3 outputDirection = Vector3.zero;
        for (int i = 0; i < Directions.DirectionCount; i++) {
            outputDirection += Directions.directions[i] * interest[i];
        }

        outputDirection.Normalize();

        var angleDanger = Vector3.Angle(outputDirection, maxDangerVector);
        var angleInterest = Vector3.Angle(outputDirection, maxInterestVector);

        // final release force and danger locks if can move to original target
        if (angleDanger >= TotalUnlockDangerAngle
            && angleInterest <= TotalUnlockInterestAngle
            && angleInterest >= 35f) {
            lockIndex = -1;
            startDangerLockIndex = -1;
            lockDangers = new float[Directions.DirectionCount];
        }

        resultTemp = outputDirection;
        interestsTemp = interest;
        dangersTemp = danger;

        //return the selected movement direction
        return outputDirection;
    }

#if UNITY_EDITOR
    private void OnDrawGizmos() {
        if (!Application.isPlaying) return;

        if (dangersTemp != null && showDangersGizmo) {
            Gizmos.color = Color.red;
            for (int i = 0; i < dangersTemp.Length; i++) {
                Vector3 direction = Directions.directions[i] * dangersTemp[i] * 2;
                var position = transform.position;
                Gizmos.DrawRay(position, direction);
                Handles.Label(position + direction, i.ToString());
            }
        }

        if (interestsTemp != null && showInterestsGizmo) {
            Gizmos.color = Color.green;
            for (int i = 0; i < interestsTemp.Length; i++) {
                Gizmos.DrawRay(transform.position, Directions.directions[i] * interestsTemp[i] * 2);
            }
        }

        if (showResultGizmo) {
            Gizmos.color = Color.yellow;
            var position = transform.position;
            var upResult = new Vector3(position.x, position.y + 0.5f, position.z);
            Gizmos.DrawRay(upResult, resultTemp * RayLength);
        }
    }
#endif

    private int StepCount(int current, int target) {
        if (rightLeft == 1) {
            return (target + Directions.DirectionCount - current) % Directions.DirectionCount;
        }
        else {
            return (current - (target - Directions.DirectionCount)) % Directions.DirectionCount;
        }
    }

    private int GetNextSideIndex(int currentIndex, int offset) =>
        (currentIndex + GetOffset(offset, rightLeft)) % Directions.DirectionCount;

    private int GetPreviousSideIndex(int currentIndex, int offset) =>
        (currentIndex + GetOffset(offset, -rightLeft)) % Directions.DirectionCount;

    private int GetOffset(int offset, int direction) =>
        (Directions.DirectionCount + offset * direction) % Directions.DirectionCount;

    private int GetIndexOfMax(float[] array) {
        if (array == null || array.Length == 0) {
            return -1; // Возвращаем -1, если массив пустой или null
        }

        int maxIndex = 0;
        float maxValue = array[0];

        for (int i = 1; i < array.Length; i++) {
            if (array[i] > maxValue) {
                maxValue = array[i];
                maxIndex = i;
            }
        }

        return maxIndex;
    }
}

public static class LockDangerExtensions {
    public static bool IsLock(this float danger) {
        return danger > 0.99;
    }
}
using UnityEngine;

namespace Pepperon.Scripts.ScriptableObjects {
[CreateAssetMenu(fileName = "UnitData", menuName = "Scriptable Objects/UnitScriptableObject", order = 1)]
public class UnitSO : ScriptableObject {
    public float agentColliderSize; // take obstacle force
    public float obstacleRange;
    public float chaseRange;
    public float targetReachedThreshold;
    public float attackRange;

    public readonly int runningAnimationSpeedName = Animator.StringToHash("runningAnimationSpeed");
    public float speed;
    public float acceleration;
    public float deacceleration;
    public float rotationSpeed;

    public float maxHealth;

    public float attack;

    public readonly int attackingAnimationSpeedName = Animator.StringToHash("attackingAnimationSpeed");
    [Range(0.01f, 10)] public float attackDelay;
    public float maxAttackAnimationDuration = 1;
    public float AttackSpeed => 1 / attackDelay; // attack per second
    public float AttackAnimationMultiplier => 1 / AttackAnimationDuration;

    public float AttackAnimationDuration =>
        attackDelay > maxAttackAnimationDuration ? maxAttackAnimationDuration : attackDelay;
}
}
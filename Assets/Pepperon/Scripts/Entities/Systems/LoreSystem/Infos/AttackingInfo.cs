using UnityEngine;

namespace Pepperon.Scripts.Entities.Systems.LoreSystem.Infos {
[CreateAssetMenu(fileName = "Attacking", menuName = "Scriptable Objects/Lore/Info/Attacking")]
public class AttackingInfo : Info {
    public readonly int attackingAnimationSpeedName = Animator.StringToHash("attackingAnimationSpeed");
    public float attack = 10f;
    public float attackRange = 1f;
    [Range(0.01f, 10)] public float attackDelay = 1f;
    public float maxAttackAnimationDuration = 1;

    public float AttackSpeed => 1 / attackDelay; // attack per second
    public float AttackAnimationMultiplier => 1 / AttackAnimationDuration;

    public float AttackAnimationDuration =>
        attackDelay > maxAttackAnimationDuration ? maxAttackAnimationDuration : attackDelay;
}
}
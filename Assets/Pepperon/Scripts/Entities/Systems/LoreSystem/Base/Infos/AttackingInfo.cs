using System;
using UnityEngine;

namespace Pepperon.Scripts.Entities.Systems.LoreSystem.Base.Infos {
[CreateAssetMenu(fileName = "Attacking", menuName = "Scriptable Objects/Lore/Info/Attacking")]
public class AttackingInfo : EntityInfo {
    public readonly int attackingAnimationSpeedName = Animator.StringToHash("attackingAnimationSpeed");
    public float attack = 10f;
    public float attackRange = 1f;
    [Range(0.01f, 10)] public float attackDelay = 1f;
    public float maxAttackAnimationDuration = 1;

    public override EntityInfoProgress ToProgress() =>
        new AttackingInfoProgress {
            attack = attack,
            attackRange = attackRange,
            attackDelay = attackDelay,
            maxAttackAnimationDuration = maxAttackAnimationDuration
        };
}

[Serializable]
public class AttackingInfoProgress : EntityInfoProgress {
    public float attack;
    public float attackRange;
    [Range(0.01f, 10)] public float attackDelay;
    public float maxAttackAnimationDuration;
    
    public float AttackSpeed => 1 / attackDelay; // attack per second
    public float AttackAnimationMultiplier => 1 / AttackAnimationDuration;

    public float AttackAnimationDuration =>
        attackDelay > maxAttackAnimationDuration ? maxAttackAnimationDuration : attackDelay;
}
}
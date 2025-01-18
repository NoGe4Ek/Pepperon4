using System;
using UnityEngine;

namespace Pepperon.Scripts.Entities.Systems.LoreSystem.Base.Infos {
[CreateAssetMenu(fileName = "RangeAttacking", menuName = "Scriptable Objects/Lore/Info/RangeAttacking")]
public class RangeAttackingInfo : AttackingInfo {
    public GameObject projectilePrefab;
    public float projectileSpeed;

    public override EntityInfoProgress ToProgress() {
        return new RangeAttackingInfoProgress() {
            attack = attack,
            attackRange = attackRange,
            attackDelay = attackDelay,
            maxAttackAnimationDuration = maxAttackAnimationDuration,
            projectileSpeed = projectileSpeed
        };
    }
}

[Serializable]
public class RangeAttackingInfoProgress : AttackingInfoProgress {
    public float projectileSpeed;
}
}
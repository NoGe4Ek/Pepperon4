using UnityEngine;

namespace Pepperon.Scripts.Entities.Systems.LoreSystem.Infos {
[CreateAssetMenu(fileName = "RangeAttacking", menuName = "Scriptable Objects/Lore/Info/RangeAttacking")]
public class RangeAttackingInfo : AttackingInfo {
    public GameObject projectilePrefab;
    public float projectileSpeed;
}
}
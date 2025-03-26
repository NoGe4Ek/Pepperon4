using System.Collections;
using Mirror;
using Pepperon.Scripts.Systems.LoreSystem.Base.Infos;
using Pepperon.Scripts.Units.Components;
using Pepperon.Scripts.Units.Components.AttackingComponents;
using UnityEngine;

namespace Pepperon.Scripts.Entities.Components.AttackingComponents {
public class RangeAttackingComponent : BaseAttackingComponent {
    private RangeAttackingInfo RangeAttackingInfo => (RangeAttackingInfo)attackingInfo;

    protected override void Awake() {
        base.Awake();

        if (!isServer) return;
        tempAttackingInfo = new TempRangeAttackingInfo();
    }

    protected override IEnumerator Attack() {
        yield return PerformAttack(() => {
            // todo move to key animation trigger
            if (isServer) {
                var projectile = Instantiate(RangeAttackingInfo.projectilePrefab, transform.position,
                    Quaternion.identity);
                NetworkServer.Spawn(projectile, connectionToClient);
                ProjectileComponent projectileComponent = projectile.GetComponent<ProjectileComponent>();
                projectileComponent.Init(attackingData.currentTarget, RangeAttackingInfo.projectileSpeed, GetAttack());
            }
        });
    }
}
}
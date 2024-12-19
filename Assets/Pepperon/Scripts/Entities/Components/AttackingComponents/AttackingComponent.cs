using System.Collections;
using Pepperon.Scripts.Managers;
using Pepperon.Scripts.Units.Components.AttackingComponents;

namespace Pepperon.Scripts.Entities.Components.AttackingComponents {
public class AttackingComponent : BaseAttackingComponent {
    protected override void Awake() {
        base.Awake();
        
        if (!isServer) return;
        tempAttackingInfo = new TempAttackingInfo();
    }

    protected override IEnumerator Attack() {
        yield return PerformAttack(() => {
            BattleManager.ApplyDamage(gameObject, attackingData.currentTarget.gameObject, GetAttack());
        });
    }
}
}
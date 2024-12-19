using Pepperon.Scripts.Entities.States._Base.Strategies;
using Pepperon.Scripts.Units.States.DyingState.Strategies.UnitStrategy;
using Pepperon.Scripts.Units.States.RunningState.Strategies.UnitStrategy;
using UnityEngine;

namespace Pepperon.Scripts.Units.States.AttackingState.Strategies.UnitStrategy {
[CreateAssetMenu(fileName = "AttackingUnit", menuName = "Scriptable Objects/Strategies/Unit/Attacking")]
public class AttackingUnitStrategy : CompositeStrategy {
    public override IServerStrategy Server => AttackingUnitServerStrategy.Instance;
    public override IClientStrategy Client => AttackingUnitClientStrategy.Instance;
}
}
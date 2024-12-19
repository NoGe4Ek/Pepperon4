using Pepperon.Scripts.Entities.States._Base.Strategies;
using UnityEngine;

namespace Pepperon.Scripts.Units.States.DyingState.Strategies.UnitStrategy {
[CreateAssetMenu(fileName = "DyingUnit", menuName = "Scriptable Objects/Strategies/Unit/Dying")]
public class DyingUnitStrategy : CompositeStrategy {
    public override IServerStrategy Server => DyingUnitServerStrategy.Instance;
    public override IClientStrategy Client => DyingUnitClientStrategy.Instance;
}
}
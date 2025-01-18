using Pepperon.Scripts.Entities.States._Base.Strategies;
using UnityEngine;

namespace Pepperon.Scripts.Entities.States.CastingState.Strategies.UnitStrategy {
[CreateAssetMenu(fileName = "CastingUnit", menuName = "Scriptable Objects/Strategies/Unit/Casting")]
public class CastingUnitStrategy : CompositeStrategy {
    public override IServerStrategy Server => CastingUnitServerStrategy.Instance;
    public override IClientStrategy Client => CastingUnitClientStrategy.Instance;
}
}
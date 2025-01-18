using Pepperon.Scripts.Entities.States._Base.Strategies;
using UnityEngine;

namespace Pepperon.Scripts.Units.States.IdleState.Strategies.UnitStrategy {
[CreateAssetMenu(fileName = "IdleUnit", menuName = "Scriptable Objects/Strategies/Unit/Idle")]
public class IdleUnitStrategy: CompositeStrategy {
    public override IServerStrategy Server => IdleUnitServerStrategy.Instance;
    public override IClientStrategy Client => IdleUnitClientStrategy.Instance;
}
}
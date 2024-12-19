using Pepperon.Scripts.Entities.States._Base.Strategies;
using UnityEngine;

namespace Pepperon.Scripts.Units.States.RunningState.Strategies.UnitStrategy {
[CreateAssetMenu(fileName = "RunningUnit", menuName = "Scriptable Objects/Strategies/Unit/Running")]
public class RunningUnitStrategy: CompositeStrategy {
    public override IServerStrategy Server => RunningUnitServerStrategy.Instance;
    public override IClientStrategy Client => RunningUnitClientStrategy.Instance;
}
}
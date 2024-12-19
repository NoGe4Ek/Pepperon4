using Pepperon.Scripts.Entities.States._Base.Strategies;
using UnityEngine;

namespace Pepperon.Scripts.Units.States.IdleState.Strategies.BuildingStrategy {
[CreateAssetMenu(fileName = "IdleBuilding", menuName = "Scriptable Objects/Strategies/Building/Idle")]
public class IdleBuildingStrategy : CompositeStrategy {
    public override IServerStrategy Server => IdleBuildingServerStrategy.Instance;
    public override IClientStrategy Client => IdleBuildingClientStrategy.Instance;
}
}
using Pepperon.Scripts.Entities.States._Base.Strategies;
using UnityEngine;

namespace Pepperon.Scripts.Units.States.DyingState.Strategies.BuildingStrategy {
[CreateAssetMenu(fileName = "DyingBuilding", menuName = "Scriptable Objects/Strategies/Building/Dying")]
public class DyingBuildingStrategy : CompositeStrategy {
    public override IServerStrategy Server => DyingBuildingServerStrategy.Instance;
    public override IClientStrategy Client => DyingBuildingClientStrategy.Instance;
}
}
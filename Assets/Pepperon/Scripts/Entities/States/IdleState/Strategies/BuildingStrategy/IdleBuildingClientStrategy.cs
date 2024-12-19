using Pepperon.Scripts.Entities.States._Base.Strategies;
using Pepperon.Scripts.Units.Components.StateMachines;

namespace Pepperon.Scripts.Units.States.IdleState.Strategies.BuildingStrategy {
public class IdleBuildingClientStrategy: IClientStrategy {
    private IdleBuildingClientStrategy() { }
    public static IdleBuildingClientStrategy Instance { get; } = new();
    public void OnEnterState(StateMachineComponent stateMachineComponent) {
        // stateMachineComponent.animationComponent.SetTrigger(Idle.TriggerName);
    }

    public void OnActiveState(StateMachineComponent stateMachineComponent) {
        // todo
    }

    public void OnExitState(StateMachineComponent stateMachineComponent) {
        // todo
    }
}
}
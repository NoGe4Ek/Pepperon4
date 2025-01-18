using Pepperon.Scripts.Entities.States._Base.Strategies;
using Pepperon.Scripts.Units.Components.StateMachines;
using Pepperon.Scripts.Units.States.RunningState;

namespace Pepperon.Scripts.Units.States.IdleState.Strategies.BuildingStrategy {
public class IdleBuildingServerStrategy : IServerStrategy {
    private IdleBuildingServerStrategy() { }
    public static IdleBuildingServerStrategy Instance { get; } = new();

    public void OnEnterState(StateMachineComponent stateMachineComponent) {
        // stateMachineComponent.animationComponent.SetTrigger(Idle.TriggerName);
    }

    public void OnActiveState(StateMachineComponent stateMachineComponent) {
        // var state = stateMachineComponent.GetStateOfType(typeof(Running));
        // stateMachineComponent.SwitchState(state);
    }

    public void OnExitState(StateMachineComponent stateMachineComponent) {
        // todo
    }
}
}
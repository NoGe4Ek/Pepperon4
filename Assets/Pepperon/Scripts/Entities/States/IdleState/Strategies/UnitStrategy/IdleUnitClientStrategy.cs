using Pepperon.Scripts.Entities.States._Base.Strategies;
using Pepperon.Scripts.Units.Components.StateMachines;

namespace Pepperon.Scripts.Units.States.IdleState.Strategies.UnitStrategy {
public class IdleUnitClientStrategy: IClientStrategy {
    private IdleUnitClientStrategy() { }
    public static IdleUnitClientStrategy Instance { get; } = new();
    public void OnEnterState(StateMachineComponent stateMachineComponent) {
        stateMachineComponent.animationComponent.SetTrigger(Idle.TriggerName);
    }

    public void OnActiveState(StateMachineComponent stateMachineComponent) {
        // todo
    }

    public void OnExitState(StateMachineComponent stateMachineComponent) {
        stateMachineComponent.animationComponent.ResetTrigger(Idle.TriggerName);
    }
}
}
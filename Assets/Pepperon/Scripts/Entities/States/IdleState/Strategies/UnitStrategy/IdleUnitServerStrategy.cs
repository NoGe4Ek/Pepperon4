using Pepperon.Scripts.Entities.States._Base.Strategies;
using Pepperon.Scripts.Units.Components.StateMachines;
using Pepperon.Scripts.Units.States.RunningState;

namespace Pepperon.Scripts.Units.States.IdleState.Strategies.UnitStrategy {
public class IdleUnitServerStrategy: IServerStrategy {
    private IdleUnitServerStrategy() { }
    public static IdleUnitServerStrategy Instance { get; } = new();
    public void OnEnterState(StateMachineComponent stateMachineComponent) {
        stateMachineComponent.animationComponent.SetTrigger(Idle.TriggerName);
    }

    public void OnActiveState(StateMachineComponent stateMachineComponent) {
        stateMachineComponent.SwitchState(RunningHolder.Instance);
    }

    public void OnExitState(StateMachineComponent stateMachineComponent) {
        stateMachineComponent.animationComponent.ResetTrigger(Idle.TriggerName);
    }
}
}
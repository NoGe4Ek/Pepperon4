using Pepperon.Scripts.Entities.States._Base.Strategies;
using Pepperon.Scripts.Units.Components.StateMachines;

namespace Pepperon.Scripts.Units.States.RunningState.Strategies.UnitStrategy {
public class RunningUnitClientStrategy: IClientStrategy {
    private RunningUnitClientStrategy() { }
    public static RunningUnitClientStrategy Instance { get; } = new();
    public void OnEnterState(StateMachineComponent stateMachineComponent) {
        stateMachineComponent.animationComponent.SetTrigger(Running.TriggerName);
    }

    public void OnActiveState(StateMachineComponent stateMachineComponent) {
        // todo
    }

    public void OnExitState(StateMachineComponent stateMachineComponent) {
        stateMachineComponent.animationComponent.ResetTrigger(Running.TriggerName);
    }
}
}
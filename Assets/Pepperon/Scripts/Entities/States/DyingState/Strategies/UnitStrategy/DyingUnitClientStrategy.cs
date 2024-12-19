using Pepperon.Scripts.Entities.States._Base.Strategies;
using Pepperon.Scripts.Units.Components.StateMachines;

namespace Pepperon.Scripts.Units.States.DyingState.Strategies.UnitStrategy {
public class DyingUnitClientStrategy: IClientStrategy {
    private DyingUnitClientStrategy() { }
    public static DyingUnitClientStrategy Instance { get; } = new();
    public void OnEnterState(StateMachineComponent stateMachineComponent) {
        stateMachineComponent.animationComponent.SetTrigger(Dying.TriggerName);
    }

    public void OnActiveState(StateMachineComponent stateMachineComponent) {
        // todo
    }

    public void OnExitState(StateMachineComponent stateMachineComponent) {
        stateMachineComponent.animationComponent.ResetTrigger(Dying.TriggerName);
    }
}
}
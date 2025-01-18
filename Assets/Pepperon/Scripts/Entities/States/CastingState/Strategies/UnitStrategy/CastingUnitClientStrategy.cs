using Pepperon.Scripts.Entities.States._Base.Strategies;
using Pepperon.Scripts.Units.Components.StateMachines;

namespace Pepperon.Scripts.Entities.States.CastingState.Strategies.UnitStrategy {
public class CastingUnitClientStrategy : IClientStrategy {
    private CastingUnitClientStrategy() { }
    public static CastingUnitClientStrategy Instance { get; } = new();

    public void OnEnterState(StateMachineComponent stateMachineComponent) {
        // stateMachineComponent.animationComponent.SetTrigger(Casting.TriggerName);
        // stateMachineComponent.animationComponent.SetTrigger(stateMachineComponent.abilityComponent.currentAbilityTrigger);
    }

    public void OnActiveState(StateMachineComponent stateMachineComponent) {
        // todo
    }

    public void OnExitState(StateMachineComponent stateMachineComponent) {
        // stateMachineComponent.animationComponent.ResetTrigger(Casting.TriggerName);
        // stateMachineComponent.animationComponent.ResetTrigger(stateMachineComponent.abilityComponent.currentAbilityTrigger);
    }
}
}
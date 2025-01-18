using Pepperon.Scripts.Entities.Components;
using Pepperon.Scripts.Entities.States._Base.Strategies;
using Pepperon.Scripts.Units.Components.StateMachines;
using UnityEngine;

namespace Pepperon.Scripts.Entities.States.CastingState.Strategies.UnitStrategy {
public class CastingUnitServerStrategy : IServerStrategy {
    private CastingUnitServerStrategy() { }
    public static CastingUnitServerStrategy Instance { get; } = new();
    
    public void OnEnterState(StateMachineComponent stateMachineComponent) {
        // stateMachineComponent.attackingComponent.Enable();
        // stateMachineComponent.animationComponent.SetTrigger(Attacking.TriggerName);
        stateMachineComponent.abilityComponent.UseAbility();
        AbilityComponent.OnCastingEnd += stateMachineComponent.OnCastingEnd;
        
        Debug.Log("Casting OnEnterState " + stateMachineComponent.transform.name);
    }

    public void OnActiveState(StateMachineComponent stateMachineComponent) {
        // todo
    }

    public void OnExitState(StateMachineComponent stateMachineComponent) {
        // stateMachineComponent.attackingComponent.Disable();
        // stateMachineComponent.animationComponent.ResetTrigger(Attacking.TriggerName);
        AbilityComponent.OnCastingEnd -= stateMachineComponent.OnCastingEnd;
        
        Debug.Log("Casting OnExitState " + stateMachineComponent.transform.name);
    }
}
}
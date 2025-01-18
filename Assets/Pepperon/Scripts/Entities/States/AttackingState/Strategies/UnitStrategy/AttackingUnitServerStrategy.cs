using System.Linq;
using Pepperon.Scripts.Entities.States._Base.Strategies;
using Pepperon.Scripts.Entities.States.CastingState;
using Pepperon.Scripts.Units.Components.StateMachines;
using Pepperon.Scripts.Units.Data;
using Pepperon.Scripts.Units.States.IdleState;
using Pepperon.Scripts.Units.States.RunningState;
using UnityEngine;

namespace Pepperon.Scripts.Units.States.AttackingState.Strategies.UnitStrategy {
public class AttackingUnitServerStrategy : IServerStrategy {
    private AttackingUnitServerStrategy() { }
    public static AttackingUnitServerStrategy Instance { get; } = new();

    public void OnEnterState(StateMachineComponent stateMachineComponent) {
        stateMachineComponent.attackingComponent.Enable();
        stateMachineComponent.animationComponent.SetTrigger(Attacking.TriggerName);
        
        Debug.Log("Attacking OnEnterState " + stateMachineComponent.transform.name);
    }

    public void OnActiveState(StateMachineComponent stateMachineComponent) {
        if (stateMachineComponent.attackingComponent.attackingData.currentTarget == null
            && !stateMachineComponent.attackingComponent.attackingData.targets.Any()) {
            stateMachineComponent.SwitchState(RunningHolder.Instance);
        }
        
        if (stateMachineComponent.abilityComponent && stateMachineComponent.abilityComponent.IsAbilityAvailable()) {
            stateMachineComponent.SwitchState(CastingHolder.Instance);
        }
        
        if (stateMachineComponent.attackingComponent.attackingState ==
            stateMachineComponent.attackingComponent.lastLocalAttackingState) return;
        stateMachineComponent.attackingComponent.lastLocalAttackingState =
            stateMachineComponent.attackingComponent.attackingState;

        switch (stateMachineComponent.attackingComponent.attackingState) {
            case AttackingData.AttackStateEnum.Attacking:
                stateMachineComponent.animationComponent.ResetTrigger(Idle.TriggerName);
                stateMachineComponent.animationComponent.SetTrigger(Attacking.TriggerName);
                break;
            case AttackingData.AttackStateEnum.WaitingForAttack:
                stateMachineComponent.animationComponent.ResetTrigger(Attacking.TriggerName);
                stateMachineComponent.animationComponent.SetTrigger(Idle.TriggerName);
                break;
        }
    }

    public void OnExitState(StateMachineComponent stateMachineComponent) {
        stateMachineComponent.attackingComponent.Disable();
        stateMachineComponent.animationComponent.ResetTrigger(Attacking.TriggerName);
        Debug.Log("Attacking OnExitState " + stateMachineComponent.transform.name);
    }
}
}
using Pepperon.Scripts.AI;
using Pepperon.Scripts.Entities.States._Base.Strategies;
using Pepperon.Scripts.Units.Components.StateMachines;
using Pepperon.Scripts.Units.Data;
using Pepperon.Scripts.Units.States.IdleState;

namespace Pepperon.Scripts.Units.States.AttackingState.Strategies.UnitStrategy {
public class AttackingUnitClientStrategy : IClientStrategy {
    private AttackingUnitClientStrategy() { }
    public static AttackingUnitClientStrategy Instance { get; } = new();

    public void OnEnterState(StateMachineComponent stateMachineComponent) {
        stateMachineComponent.animationComponent.SetTrigger(Attacking.TriggerName);
    }

    public void OnActiveState(StateMachineComponent stateMachineComponent) {
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
        stateMachineComponent.animationComponent.ResetTrigger(Attacking.TriggerName);
    }
}
}
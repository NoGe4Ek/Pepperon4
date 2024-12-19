using System.Linq;
using Pepperon.Scripts.Entities.States._Base.Strategies;
using Pepperon.Scripts.Units.Components.StateMachines;
using Pepperon.Scripts.Units.States.RunningState;
using UnityEngine;

namespace Pepperon.Scripts.Units.States.AttackingState.Strategies.UnitStrategy {
public class AttackingUnitServerStrategy : IServerStrategy {
    private AttackingUnitServerStrategy() { }
    public static AttackingUnitServerStrategy Instance { get; } = new();

    public void OnEnterState(StateMachineComponent stateMachineComponent) {
        stateMachineComponent.attackingComponent.Enable();
        stateMachineComponent.animationComponent.SetTrigger(Attacking.TriggerName);
        
        // Debug.Log("Attacking OnEnterState " + stateMachineComponent.transform.name);
    }

    public void OnActiveState(StateMachineComponent stateMachineComponent) {
        if (stateMachineComponent.attackingComponent.attackingData.currentTarget == null
            && !stateMachineComponent.attackingComponent.attackingData.targets.Any()) {
            stateMachineComponent.SwitchState(RunningHolder.Instance);
        }
    }

    public void OnExitState(StateMachineComponent stateMachineComponent) {
        stateMachineComponent.attackingComponent.Disable();
        stateMachineComponent.animationComponent.ResetTrigger(Attacking.TriggerName);
        // Debug.Log("Attacking OnExitState " + stateMachineComponent.transform.name);
    }
}
}
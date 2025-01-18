using Pepperon.Scripts.Entities.States._Base.Strategies;
using Pepperon.Scripts.Units.Components.StateMachines;
using Pepperon.Scripts.Units.Data;
using Pepperon.Scripts.Units.States.AttackingState;
using UnityEngine;

namespace Pepperon.Scripts.Units.States.RunningState.Strategies.UnitStrategy {
public class RunningUnitServerStrategy: IServerStrategy {
    private RunningUnitServerStrategy() { }
    public static RunningUnitServerStrategy Instance { get; } = new();
    public void OnEnterState(StateMachineComponent stateMachineComponent) {
        stateMachineComponent.movementComponent.isActive = true;
        stateMachineComponent.attackingComponent.Enable();
        
        stateMachineComponent.animationComponent.SetTrigger(Running.TriggerName);
        stateMachineComponent.movementComponent.isChasing = true;
        
        // Debug.Log("Running OnEnterState " + stateMachineComponent.transform.name);
    }

    public void OnActiveState(StateMachineComponent stateMachineComponent) {
        if (stateMachineComponent.movementComponent.movementData.MovementState != MovementData.MovementStateEnum.NotMoving) {
            if (stateMachineComponent.attackingComponent.attackingData.currentTarget) {
                // stateMachineComponent.attackingComponent.attackingData.currentTarget = stateMachineComponent.attackingComponent.attackingData.targets.FirstOrDefault();
                stateMachineComponent.movementComponent.MovementDirection = Vector3.zero;
                stateMachineComponent.movementComponent.isChasing = false;
                stateMachineComponent.SwitchState(AttackingStateHolder.Instance);
            }
        }
    }

    public void OnExitState(StateMachineComponent stateMachineComponent) {
        stateMachineComponent.StopAllCoroutines();
        stateMachineComponent.movementComponent.isChasing = false;

        stateMachineComponent.movementComponent.Disable();
        stateMachineComponent.attackingComponent.Disable();
        
        stateMachineComponent.animationComponent.ResetTrigger(Running.TriggerName);
        // Debug.Log("Running OnExitState " + stateMachineComponent.transform.name);
    }
}
}
using System.Collections;
using Pepperon.Scripts.Entities.States._Base.Strategies;
using Pepperon.Scripts.Units.Components.StateMachines;
using UnityEngine;

namespace Pepperon.Scripts.Units.States.DyingState.Strategies.UnitStrategy {
public class DyingUnitServerStrategy : IServerStrategy {
    private DyingUnitServerStrategy() { }
    public static DyingUnitServerStrategy Instance { get; } = new();

    public void OnEnterState(StateMachineComponent stateMachineComponent) {
        stateMachineComponent.animationComponent.SetTrigger(Dying.TriggerName);

        var gameObject = stateMachineComponent.gameObject;
        var position = gameObject.transform.position;
        stateMachineComponent.attackingComponent.attackingData.enabled = false;
        stateMachineComponent.movementComponent.movementData.enabled = false;
        gameObject.transform.TryGetComponent(out CapsuleCollider collider);
        collider.center = new Vector3(position.x, -100f, position.z);
    }

    public void OnActiveState(StateMachineComponent stateMachineComponent) {
        WaitBeforeDying(stateMachineComponent);
    }

    public void OnExitState(StateMachineComponent stateMachineComponent) {
        stateMachineComponent.animationComponent.ResetTrigger(Dying.TriggerName);
        
        var gameObject = stateMachineComponent.gameObject;
        var position = gameObject.transform.position;
        gameObject.transform.position = new Vector3(position.x, -100f, position.z);
        Object.Destroy(gameObject, 3f);
    }

    private void WaitBeforeDying(StateMachineComponent stateMachineComponent) {
        stateMachineComponent.StartCoroutine(WaitAndDye(stateMachineComponent));
    }

    private IEnumerator WaitAndDye(StateMachineComponent stateMachineComponent) {
        yield return new WaitForSeconds(3);
        stateMachineComponent.SwitchState(DyingHolder.Instance);
    }
}
}
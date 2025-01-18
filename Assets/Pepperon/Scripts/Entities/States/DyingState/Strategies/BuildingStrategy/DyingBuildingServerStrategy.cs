using System.Collections;
using Pepperon.Scripts.Entities.States._Base.Strategies;
using Pepperon.Scripts.Units.Components.StateMachines;
using UnityEngine;

namespace Pepperon.Scripts.Units.States.DyingState.Strategies.BuildingStrategy {
public class DyingBuildingServerStrategy : IServerStrategy {
    private DyingBuildingServerStrategy() { }
    public static DyingBuildingServerStrategy Instance { get; } = new();

    public void OnEnterState(StateMachineComponent stateMachineComponent) {
        // stateMachineComponent.animationComponent.SetTrigger(Dying.TriggerName);
    }

    public void OnActiveState(StateMachineComponent stateMachineComponent) {
        stateMachineComponent.SwitchState(DyingHolder.Instance);
    }

    public void OnExitState(StateMachineComponent stateMachineComponent) {
        var gameObject = stateMachineComponent.gameObject;
        var position = gameObject.transform.position;
        gameObject.transform.position = new Vector3(position.x, -100f, position.z);
        Object.Destroy(gameObject, 3f);
    }
}
}
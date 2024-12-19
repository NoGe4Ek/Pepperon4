using Pepperon.Scripts.Units.Components.StateMachines;
using UnityEngine;

namespace Pepperon.Scripts.Entities.States._Base.Strategies {
public abstract class CompositeStrategy : ScriptableObject {
    public abstract IServerStrategy Server { get; }
    public abstract IClientStrategy Client { get; }

    public void OnEnterState(StateMachineComponent stateMachineComponent) {
        if (stateMachineComponent.isServer) Server.OnEnterState(stateMachineComponent);
        if (stateMachineComponent.isClient) Client.OnEnterState(stateMachineComponent);
    }

    public void OnActiveState(StateMachineComponent stateMachineComponent) {
        if (stateMachineComponent.isServer) Server.OnActiveState(stateMachineComponent);
        if (stateMachineComponent.isClient) Client.OnActiveState(stateMachineComponent);
    }

    public void OnExitState(StateMachineComponent stateMachineComponent) {
        if (stateMachineComponent.isServer) Server.OnExitState(stateMachineComponent);
        if (stateMachineComponent.isClient) Client.OnExitState(stateMachineComponent);
    }
}
}
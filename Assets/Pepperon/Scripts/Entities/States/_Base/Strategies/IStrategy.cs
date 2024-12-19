using Pepperon.Scripts.Units.Components.StateMachines;

namespace Pepperon.Scripts.Entities.States._Base.Strategies {
public interface IStrategy {
    public void OnEnterState(StateMachineComponent stateMachineComponent);
    public void OnActiveState(StateMachineComponent stateMachineComponent);
    public void OnExitState(StateMachineComponent stateMachineComponent);
}
}
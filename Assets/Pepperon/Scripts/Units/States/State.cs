using Pepperon.Scripts.Units.Managers;

namespace Pepperon.Scripts.AI.Units.States {
public abstract class State {
    public abstract string TriggerName { get; }

    public abstract void EnterState(StateManager stateManager);
    public abstract void Update(StateManager stateManager);
    public abstract void ExitState(StateManager stateManager);
}
}
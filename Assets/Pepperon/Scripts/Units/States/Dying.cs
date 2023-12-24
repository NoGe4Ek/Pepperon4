using Pepperon.Scripts.Units.Managers;

namespace Pepperon.Scripts.AI.Units.States {
public class Dying : State {
    public override string TriggerName => "ToDying";

    public override void EnterState(StateManager stateManager) {
        stateManager.animator.SetTrigger(TriggerName);
    }

    public override void Update(StateManager stateManager) {
        stateManager.WaitAndSwitchState(new Idle());
    }

    public override void ExitState(StateManager stateManager) {
        // todo
    }
}
}
using Pepperon.Scripts.Units.Managers;
using Pepperon.Scripts.Units.States;

namespace Pepperon.Scripts.AI.Units.States {
public class Idle : State {
    public override string TriggerName => "ToIdle";

    public override void EnterState(StateManager stateManager) {
        stateManager.animator.SetTrigger(TriggerName);
    }

    public override void Update(StateManager stateManager) {
        stateManager.SwitchState(new Running());
        //stateManager.WaitAndSwitchState(new Running());
    }

    public override void ExitState(StateManager stateManager) {
        // todo
    }
}
}
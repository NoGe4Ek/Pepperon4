using System.Collections;
using Pepperon.Scripts.AI;
using Pepperon.Scripts.AI.Units.States;
using Pepperon.Scripts.Units.Managers;
using UnityEngine;

namespace Pepperon.Scripts.Units.States {
public class Running : State {
    private bool isChasing = true;
    
    public override string TriggerName => "ToRunning";

    public override void EnterState(StateManager stateManager) {
        stateManager.animator.SetTrigger(TriggerName);
    }

    public override void Update(StateManager stateManager) {
        //stateManager.WaitAndSwitchState(new Attacking());

        //Enemy AI movement based on Target availability
        if (stateManager.movementData.MovementState != MovementData.MovementStateEnum.NotMoving) {
            //Looking at the Target
            stateManager.onPointerInput?.Invoke(stateManager.movementInput);

            stateManager.StartCoroutine(ChaseAndAttack(stateManager));
        }

        //Moving the Agent
        stateManager.onMovementInput?.Invoke(stateManager.movementInput);
    }

    public override void ExitState(StateManager stateManager) {
        stateManager.StopAllCoroutines();
        isChasing = false;
    }

    private IEnumerator ChaseAndAttack(StateManager stateManager) {
        if (stateManager.attackingData.currentTarget != null) {
            stateManager.movementInput = Vector3.zero;
            stateManager.SwitchState(new Attacking());
        }

        // if (distance < attackDistance)
        // {
        //     //Attack logic
        //     movementInput = Vector3.zero;
        //     OnAttackPressed?.Invoke();
        //     yield return new WaitForSeconds(attackDelay);
        //     StartCoroutine(ChaseAndAttack());
        // }
        // else
        // {
        //Chase logic
        if (isChasing) {
            stateManager.movementInput =
                stateManager.movementDirectionSolver.GetDirectionToMove(stateManager.behaviours);
            yield return new WaitForSeconds(ProjectSettingsManager.Instance.aiUpdateDelay);
        }
        //stateManager.StartCoroutine(ChaseAndAttack(stateManager));
        // //}
        // if (stateManager.movementData.CurrentTarget == null) {
        //     //Stopping Logic
        //     //Debug.Log("Stopping");
        //     //stateManager.movementInput = Vector3.zero;
        //     //following = false;
        //     yield break;
        // }
        // else {
        //     
        // }
    }
}
}
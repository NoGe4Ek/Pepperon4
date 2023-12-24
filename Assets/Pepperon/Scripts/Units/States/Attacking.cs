using System.Collections;
using Pepperon.Scripts.AI.Units.Controllers;
using Pepperon.Scripts.Units.Managers;
using UnityEngine;

namespace Pepperon.Scripts.AI.Units.States {
public class Attacking : State {
    public override string TriggerName => "ToAttacking";
    private AttackStateEnum attackingState;

    public override void EnterState(StateManager stateManager) {
        // todo
    }
    
    private bool canAttack = true;
    public override void Update(StateManager stateManager) {
        //stateManager.WaitAndSwitchState(new Dying());
        if (canAttack) {
            stateManager.StartCoroutine(Attack(stateManager));
            //stateManager.StartCoroutine(AttackWait(stateManager));
        }
        
        stateManager.onPointerInput?.Invoke(stateManager.attackingData.currentTarget.position - stateManager.transform.position);
    }
    
    private enum AttackStateEnum {
        Attacking,
        WaitingForAttack
    }
    private IEnumerator Attack(StateManager stateManager) {
        stateManager.animator.SetTrigger(TriggerName);
        attackingState = AttackStateEnum.Attacking;
        stateManager.attackingData.currentTarget.GetComponentInParent<UnitController>().currentHealth -= stateManager.unit.attack;
        canAttack = false;
        
        float timer = 0f;
        while (timer < stateManager.unit.attackDelay) {
            timer += Time.deltaTime;
            if (timer > stateManager.unit.AttackAnimationDuration && attackingState != AttackStateEnum.WaitingForAttack) {
                stateManager.animator.SetTrigger(new Idle().TriggerName);
                attackingState = AttackStateEnum.WaitingForAttack;
            }
            yield return null;
        }
        stateManager.animator.ResetTrigger(TriggerName);
        stateManager.animator.ResetTrigger(new Idle().TriggerName);
        canAttack = true;
    }

    public override void ExitState(StateManager stateManager) {
        // todo
    }
}
}
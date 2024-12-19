using System;
using System.Collections;
using System.Linq;
using Mirror;
using Pepperon.Scripts.EditorExtensions.Attributes;
using Pepperon.Scripts.Entities.Components.AttackingComponents;
using Pepperon.Scripts.Entities.Controllers;
using Pepperon.Scripts.Entities.Systems.LoreSystem;
using Pepperon.Scripts.Entities.Systems.LoreSystem.Infos;
using Pepperon.Scripts.ScriptableObjects;
using Pepperon.Scripts.Units.Data;
using Pepperon.Scripts.Units.States.AttackingState;
using Pepperon.Scripts.Units.States.IdleState;
using Pepperon.Scripts.Utils;
using UnityEngine;
using UnityEngine.Serialization;

namespace Pepperon.Scripts.Units.Components.AttackingComponents {
// todo try to not network behaviour
public abstract class BaseAttackingComponent: NetworkBehaviour{
    private bool isActive = true;

    public void Disable() {
        isActive = false;
        attackingData.currentAttackCoroutine?.Pause();
        animationComponent.ResetTrigger(Attacking.TriggerName);
        animationComponent.ResetTrigger(Idle.TriggerName);
    }

    public void Enable() {
        isActive = true;
        attackingData.currentAttackCoroutine?.Unpause();
    }


    [SerializeField] protected AttackingInfo attackingInfo;
    [SerializeField] public TempAttackingInfo tempAttackingInfo;

    protected float GetAttack() => attackingInfo.attack + tempAttackingInfo.attackDelta;
    
    [SerializeField] public AttackingData attackingData;
    [SerializeField] protected Transform attackRange;
    [SyncVar] public AttackingData.AttackStateEnum attackingState;
    private void SetAttackingState(AttackingData.AttackStateEnum value) { this.attackingState = value; }
    public AttackingData.AttackStateEnum lastLocalAttackingState;
    public bool canAttack = true;

    private AnimationComponent animationComponent;

    [SerializeField] private bool shouldRotateToTarget;

    [ConditionalDisplay(nameof(shouldRotateToTarget)), SerializeField]
    private RotationComponent rotationComponent;

    protected virtual void Awake() {
        animationComponent = GetComponent<AnimationComponent>();
        attackRange.GetComponent<Renderer>().enabled = false;
        attackingInfo = GetComponent<EntityController>().entity.info.OfType<AttackingInfo>().First();
    }

    protected virtual void Update() {
        if (!isServer) return;
        if (!isActive) return;

        attackRange.localScale = new Vector3(attackingInfo.attackRange, attackingInfo.attackRange, attackingInfo.attackRange);
        animationComponent.SetFloat(attackingInfo.attackingAnimationSpeedName, attackingInfo.AttackAnimationMultiplier);

        if (attackingData.currentTarget) {
            if (canAttack && attackingData.currentAttackCoroutine is not { Running: true }) {
                attackingData.currentAttackCoroutine = new Task(Attack());
            }
        }
        else {
            if (attackingData.targets.Any()) {
                attackingData.currentTarget = attackingData.targets.FirstOrDefault();
            }
        }

        if (shouldRotateToTarget && attackingData.currentTarget)
            rotationComponent.RotationDirection = attackingData.currentTarget.position - transform.position;
    }

    protected abstract IEnumerator Attack();

    protected IEnumerator PerformAttack(Action onAttackComplete) {
        animationComponent.SetTrigger(Attacking.TriggerName);
        SetAttackingState(AttackingData.AttackStateEnum.Attacking);

        canAttack = false;
        float timer = 0f;
        while (timer < attackingInfo.attackDelay) {
            timer += Time.deltaTime;
            if (timer > attackingInfo.AttackAnimationDuration &&
                attackingState != AttackingData.AttackStateEnum.WaitingForAttack) {
                if (attackingData.currentTarget != null)
                    onAttackComplete.Invoke();
                animationComponent.ResetTrigger(Attacking.TriggerName);
                animationComponent.SetTrigger(Idle.TriggerName);
                SetAttackingState(AttackingData.AttackStateEnum.WaitingForAttack);
            }

            yield return null;
        }

        animationComponent.ResetTrigger(Attacking.TriggerName);
        animationComponent.ResetTrigger(Idle.TriggerName);
        canAttack = true;
    }
}
}
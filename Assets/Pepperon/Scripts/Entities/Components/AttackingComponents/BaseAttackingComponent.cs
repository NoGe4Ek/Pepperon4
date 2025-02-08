using System;
using System.Collections;
using System.Linq;
using Mirror;
using Pepperon.Scripts.Entities.Components.AttackingComponents;
using Pepperon.Scripts.Entities.Controllers;
using Pepperon.Scripts.Entities.Systems.LoreSystem.Base.Infos;
using Pepperon.Scripts.Units.Data;
using Pepperon.Scripts.Utils;
using UnityEngine;

namespace Pepperon.Scripts.Units.Components.AttackingComponents {
// todo try to not network behaviour
public abstract class BaseAttackingComponent : NetworkBehaviour {
    private bool isActive = true;

    public void Disable() {
        isActive = false;
        attackingData.currentAttackCoroutine?.Pause();
        // animationComponent.ResetTrigger(Attacking.TriggerName);
        // animationComponent.ResetTrigger(Idle.TriggerName);
    }

    public void Enable() {
        isActive = true;
        attackingData.currentAttackCoroutine?.Unpause();
    }


    [SerializeField] protected AttackingInfo attackingInfo;
    [SerializeField] protected AttackingInfoProgress attackingInfoProgress;
    [SerializeField] public TempAttackingInfo tempAttackingInfo;

    protected float GetAttack() => attackingInfoProgress.attack + tempAttackingInfo.attackDelta;

    [SerializeField] public AttackingData attackingData;
    [SerializeField] protected Transform attackRange;
    [SyncVar] public AttackingData.AttackStateEnum attackingState;

    private void SetAttackingState(AttackingData.AttackStateEnum value) {
        this.attackingState = value;
    }

    public AttackingData.AttackStateEnum lastLocalAttackingState;
    public bool canAttack = true;

    public event Action OnAttackPerformed;

    private AnimationComponent animationComponent;

    [SerializeField] private bool shouldRotateToTarget;

    // [ConditionalDisplay(nameof(shouldRotateToTarget)),
    [SerializeField] private RotationComponent rotationComponent;

    protected virtual void Awake() {
        animationComponent = GetComponent<AnimationComponent>();
        attackRange.GetComponent<Renderer>().enabled = false;
    }

    private void Start() {
        attackingInfo = GetComponent<EntityController>().entity.info.OfType<AttackingInfo>().First();
        attackingInfoProgress =
            GetComponent<EntityController>().entityProgress.info.OfType<AttackingInfoProgress>().First();
    }

    protected virtual void Update() {
        if (!isServer) return;
        if (!isActive) return;

        attackRange.localScale = new Vector3(attackingInfoProgress.attackRange, attackingInfoProgress.attackRange,
            attackingInfoProgress.attackRange);
        animationComponent.SetFloat(attackingInfo.attackingAnimationSpeedName,
            attackingInfoProgress.AttackAnimationMultiplier);

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
        // animationComponent.SetTrigger(Attacking.TriggerName);
        SetAttackingState(AttackingData.AttackStateEnum.Attacking);

        canAttack = false;
        float timer = 0f;
        while (timer < attackingInfoProgress.attackDelay) {
            timer += Time.deltaTime;
            if (timer > attackingInfoProgress.AttackAnimationDuration &&
                attackingState != AttackingData.AttackStateEnum.WaitingForAttack) {
                if (attackingData.currentTarget != null) {
                    onAttackComplete.Invoke();
                    OnAttackPerformed?.Invoke();
                }

                // animationComponent.ResetTrigger(Attacking.TriggerName);
                // animationComponent.SetTrigger(Idle.TriggerName);
                SetAttackingState(AttackingData.AttackStateEnum.WaitingForAttack);
            }

            yield return null;
        }

        // animationComponent.ResetTrigger(Attacking.TriggerName);
        // animationComponent.ResetTrigger(Idle.TriggerName);
        canAttack = true;
    }
}
}
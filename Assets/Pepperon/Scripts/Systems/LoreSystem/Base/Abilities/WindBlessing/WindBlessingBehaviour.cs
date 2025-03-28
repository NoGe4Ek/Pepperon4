using System.Collections;
using System.Linq;
using Pepperon.Scripts.Entities.Controllers;
using Pepperon.Scripts.Units.Components.AttackingComponents;
using Pepperon.Scripts.Utils;
using UnityEngine;
using UnityEngine.VFX;

namespace Pepperon.Scripts.Entities.Systems.LoreSystem.Base.Abilities.WindBlessing {
public class WindBlessingBehaviour : AbilityBehaviour {
    private BaseAttackingComponent attackingComponent;
    private AttackBoostAbilityInfo abilityInfo;
    private AttackBoostAbilityInfoProgress abilityInfoProgress;

    private int currentAttackDelta;
    private int currentAttackCount; // charges
    public VisualEffect effect;

    private void Awake() {
        effect.Stop();
        enabled = false;
    }

    public override bool IsAbilityNeeded() => true; // todo

    public override void Activate() {
        enabled = true;
        Debug.Log("WindBlessingBehaviour Activate");
    }

    private void Start() {
        Debug.Log("WindBlessingBehaviour Start");
        attackingComponent = GetComponent<BaseAttackingComponent>();
        var entityController = GetComponent<EntityController>();
        abilityInfo =
            entityController.entity.abilities.First(it => it.BehaviourType == typeof(WindBlessingBehaviour)).info as
                AttackBoostAbilityInfo;
        abilityInfoProgress =
            entityController.entityProgress().abilities.First(it => it.BehaviourType == typeof(WindBlessingBehaviour))
                .progress as AttackBoostAbilityInfoProgress;

        attackingComponent.OnAttackPerformed += () => {
            if (currentAttackCount == 0) return;
            currentAttackCount--;
            if (currentAttackCount == 0) {
                attackingComponent.tempAttackingInfo.attackDelta -= currentAttackDelta;
                effect.Stop();
            }
        };
    }

    private void Update() {
        if (currentAbilityCoroutine is not { Running: true }) {
            currentAbilityCoroutine = new Task(UseAbility());
        }
    }

    private IEnumerator UseAbility() {
        Debug.Log("WindBlessingBehaviour UseAbility");
        var cooldown = abilityInfo.cooldownProgresses[abilityInfoProgress.currentAbilityLevel];
        currentAttackDelta = abilityInfo.attackProgressDeltas[abilityInfoProgress.currentAbilityLevel];
        attackingComponent.tempAttackingInfo.attackDelta += currentAttackDelta;
        currentAttackCount = abilityInfo.attackCountProgresses[abilityInfoProgress.currentAbilityLevel];
        effect.Play();
        yield return new WaitForSeconds(cooldown);
    }
}

public abstract class AbilityBehaviour : MonoBehaviour {
    protected Task currentAbilityCoroutine;

    public bool IsAbilityCooldown() => currentAbilityCoroutine is { Running: true };
    public abstract bool IsAbilityNeeded();
    public abstract void Activate();
}
}
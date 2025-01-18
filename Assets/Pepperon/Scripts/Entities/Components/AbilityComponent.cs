using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Mirror;
using Pepperon.Scripts.Entities.Controllers;
using Pepperon.Scripts.Entities.Systems.LoreSystem.Base.Abilities;
using Pepperon.Scripts.Entities.Systems.LoreSystem.Base.Abilities.WindBlessing;
using Pepperon.Scripts.Units.Components;
using Pepperon.Scripts.Utils;
using UnityEngine;

namespace Pepperon.Scripts.Entities.Components {
public class AbilityComponent : NetworkBehaviour {
    public static event Action OnCastingEnd;

    private List<Ability> abilities;
    private List<AbilityProgress> abilitiesProgress;
    private AbilityBehaviour abilityBehaviour;
    private Ability currentAbility;
    [SyncVar(hook = nameof(OnAbilityCast))]
    private string currentAbilityTrigger;
    
    private void OnAbilityCast(string oldAbilityTrigger, string newAbilityTrigger) {
        if (newAbilityTrigger == "") return;
        new Task(PlayCastingAnimation(newAbilityTrigger));
    }
    
    private AnimationComponent animationComponent;


    private void Awake() {
        animationComponent = GetComponent<AnimationComponent>();
    }

    private void Start() {
        abilities = GetComponent<EntityController>().entity.abilities;
        abilitiesProgress = GetComponent<EntityController>().entityProgress.abilities;

        // var behaviourType = abilities.First().BehaviourType;
        // abilityBehaviour = GetComponent(behaviourType) as AbilityBehaviour;
        // abilityBehaviour.Activate();
    }

    public void UseAbility() {
        abilityBehaviour.Activate();
        new Task(PlayCastingAnimation(currentAbilityTrigger));
    }

    private IEnumerator PlayCastingAnimation(string abilityTrigger) {
        animationComponent.SetTrigger(abilityTrigger);

        // Wait one frame to let the animator enter the next animation first
        yield return new WaitUntil(() =>
            animationComponent.animator.GetCurrentAnimatorClipInfo(0)[0].clip.name == "Human_Hero_01_Ability_01");

        // From here, wait for the new animation to finish
        // - animation must be on first layer
        // - animation must have 0 transition time, else we must also check !m_Animator.IsInTransition(0)
        // https://answers.unity.com/questions/362629/how-can-i-check-if-an-animation-is-being-played-or.html
        yield return new WaitUntil(() => animationComponent.NormalizedTime() >= 1f);
        OnCastingEnd?.Invoke();

        animationComponent.ResetTrigger(abilityTrigger);

        currentAbility = null;
        currentAbilityTrigger = "";
        abilityBehaviour = null;
    }

    public bool IsAbilityAvailable() {
        if (currentAbility) return false;

        foreach (var ability in abilities) {
            var behaviourType = ability.BehaviourType;
            abilityBehaviour = GetComponent(behaviourType) as AbilityBehaviour;
            if (abilityBehaviour && !abilityBehaviour.IsAbilityCooldown() && abilityBehaviour.IsAbilityNeeded()) {
                currentAbility = ability;
                currentAbilityTrigger = ability.triggerName;
                return true;
            }
        }

        return false;
    }
}
}
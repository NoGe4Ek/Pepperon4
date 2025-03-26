using System.Linq;
using Pepperon.Scripts.Entities.Components;
using Pepperon.Scripts.Entities.Controllers;
using Pepperon.Scripts.Systems.LoreSystem.Base.Infos;
using Pepperon.Scripts.Utils;
using UnityEngine;

namespace Pepperon.Scripts.Entities.Detectors {
public class ExperienceDetector : MonoBehaviour {
    public void OnExperienceTriggerExit(OnTriggerDelegation delegation) {
        if (delegation.Caller.name != "ExperienceRange") return;
        var experienceComponent = delegation.Caller.GetComponentInParent<ExperienceComponent>();
        if (!experienceComponent) return;
        if (!delegation.Other.TryGetComponent(out HealthComponent healthComponent)
            || !healthComponent.isDied) return;
        if (!delegation.Other.TryGetComponent(out EntityController entityController)) return;
        var experienceInfo = entityController.entity.info.FirstOrDefault(it => it is ExperienceInfo) as ExperienceInfo;
        if (!experienceInfo) return;

        experienceComponent.TakeExperience(experienceInfo.experienceReward);
    }
}
}
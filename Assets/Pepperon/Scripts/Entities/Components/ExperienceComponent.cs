using System;
using System.Linq;
using Mirror;
using Pepperon.Scripts.Entities.Controllers;
using Pepperon.Scripts.Systems.LoreSystem.Base.Entities;
using Pepperon.Scripts.Systems.LoreSystem.Base.Infos;
using TMPro;
using UnityEngine;

namespace Pepperon.Scripts.Entities.Components {
public class ExperienceComponent : NetworkBehaviour {
    private ExperienceInfo experienceInfo;

    private ExperienceInfoProgress experienceInfoProgress =>
        GetComponent<EntityController>().entityProgress.info.OfType<ExperienceInfoProgress>().First();
    
    [SerializeField] private TMP_Text levelTextView;

    public static event Action<int, Hero, int> OnNewLevel;

    private void Start() {
        levelTextView.text = "Level: " + experienceInfoProgress.level;
        experienceInfo =
            GetComponent<EntityController>().entity.info.OfType<ExperienceInfo>().First();
    }

    public void TakeExperience(int experience) {
        TakeExperienceInternal(experience);
        RpcTakeExperience(experience);
    }

    [ClientRpc]
    private void RpcTakeExperience(int experience) {
        TakeExperienceInternal(experience);
    }

    private void TakeExperienceInternal(int experience) {
        if (experienceInfoProgress.experience >= experienceInfo.levelThresholds[^1]) return;
        experienceInfoProgress.experience += experience;

        // Check for new level
        if (experienceInfoProgress.experience < experienceInfo.levelThresholds[experienceInfoProgress.level]) return;

        // Calculate how much new levels
        for (var i = experienceInfoProgress.level; i < experienceInfo.levelThresholds.Length - 1; i++) {
            if (experienceInfoProgress.experience < experienceInfo.levelThresholds[experienceInfoProgress.level]) break;
            experienceInfoProgress.level++;
        }

        levelTextView.text = "Level: " + experienceInfoProgress.level;
        OnNewLevel?.Invoke(
            GetComponent<EntityController>().playerType,
            GetComponent<EntityController>().entity as Hero,
            experienceInfoProgress.level
        );
    }
}
}
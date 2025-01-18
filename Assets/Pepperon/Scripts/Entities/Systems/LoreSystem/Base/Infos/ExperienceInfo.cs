using System;
using UnityEngine;

namespace Pepperon.Scripts.Entities.Systems.LoreSystem.Base.Infos {
[CreateAssetMenu(fileName = "Experience", menuName = "Scriptable Objects/Lore/Info/Experience")]
public class ExperienceInfo : EntityInfo {
    public int experienceReward;
    public int[] levelThresholds;

    public override EntityInfoProgress ToProgress() {
        return new ExperienceInfoProgress();
    }
}

[Serializable]
public class ExperienceInfoProgress : EntityInfoProgress { }
}
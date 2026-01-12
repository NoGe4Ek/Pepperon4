using System;
using Pepperon.Scripts.Systems.LoreSystem.Base.Infos.EntityInfos;
using UnityEngine;

namespace Pepperon.Scripts.Systems.LoreSystem.Base.Infos {
[CreateAssetMenu(fileName = "Experience", menuName = "Scriptable Objects/Lore/Info/Experience")]
public class ExperienceInfo : EntityInfo {
    public int level = 1;
    public int experienceReward;
    public int[] levelThresholds;

    public override EntityInfoProgress ToProgress() {
        return new ExperienceInfoProgress {
            level = level,
            experience = 0
        };
    }
}

[Serializable]
public class ExperienceInfoProgress : EntityInfoProgress {
    public int level;
    public int experience;
}
}
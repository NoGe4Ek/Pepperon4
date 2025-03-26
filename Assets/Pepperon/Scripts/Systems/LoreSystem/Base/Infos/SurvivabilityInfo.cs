using System;
using Pepperon.Scripts.Systems.LoreSystem.Base.Infos.EntityInfos;
using UnityEngine;

namespace Pepperon.Scripts.Systems.LoreSystem.Base.Infos {
[CreateAssetMenu(fileName = "Survivability", menuName = "Scriptable Objects/Lore/Info/Survivability")]
public class SurvivabilityInfo : EntityInfo {
    public float maxHealth = 100f;
    
    public override EntityInfoProgress ToProgress() {
        return new SurvivabilityInfoProgress() {
            maxHealth = maxHealth
        };
    }
}

[Serializable]
public class SurvivabilityInfoProgress : EntityInfoProgress {
    public float maxHealth;
}
}
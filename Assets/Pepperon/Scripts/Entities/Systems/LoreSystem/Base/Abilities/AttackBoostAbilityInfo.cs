using UnityEngine;

namespace Pepperon.Scripts.Entities.Systems.LoreSystem.Base.Abilities {
[CreateAssetMenu(fileName = "AttackBoost", menuName = "Scriptable Objects/Lore/Info/Ability Info/Attack Boost")]
public class AttackBoostAbilityInfo : AbilityInfo {
    public int[] attackProgressDeltas;
    public int[] attackCountProgresses;
    public int[] cooldownProgresses;

    public override AbilityInfoProgress ToProgress() => new AttackBoostAbilityInfoProgress();
}

public class AttackBoostAbilityInfoProgress : AbilityInfoProgress {
    public int currentAbilityLevel;
}
}
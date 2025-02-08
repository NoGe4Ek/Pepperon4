using System;
using UnityEngine;

namespace Pepperon.Scripts.Entities.Systems.LoreSystem.Base.Upgrades {
public enum CommonUpgradeType {
    Melee,
    Range,
    Mage,
    Survivability,
    Buildings
}

[CreateAssetMenu(fileName = "Upgrade", menuName = "Scriptable Objects/Lore/Upgrades/Upgrade")]
public class Upgrade : ScriptableObject, IProgressable<UpgradeProgress> {
    public string upgradeName;
    public string upgradeDescription;
    public int[] progressDeltas;
    public int[] progressCosts;

    public UpgradeProgress ToProgress() => new();
}

[Serializable]
public class UpgradeProgress : Progress {
    public int progress = 0;
}
}
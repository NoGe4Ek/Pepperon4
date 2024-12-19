using System;

namespace Pepperon.Scripts.Entities.Systems.LoreSystem.Upgrades {
[Serializable]
public class CommonUpgrade : Upgrade {
    public CommonUpgrade(string name, string description, int[] progress) : base(name, description, progress) { }
    public CommonUpgradeType commonUpgradeType;
}

public enum CommonUpgradeType {
    Melee,
    Range,
    Mage,
    Survivability,
    Buildings
}
}
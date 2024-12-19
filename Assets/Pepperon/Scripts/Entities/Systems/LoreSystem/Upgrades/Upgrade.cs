using System;

namespace Pepperon.Scripts.Entities.Systems.LoreSystem.Upgrades {
[Serializable]
public class Upgrade {
    public Upgrade(string name, string description, int[] progressDeltas) {
        this.name = name;
        this.description = description;
        this.progressDeltas = progressDeltas;
    }

    public string name;
    public string description;
    public int progress = 0;
    public int[] progressDeltas;
}
}
using System.Collections.Generic;
using UnityEngine;

namespace Pepperon.Scripts.Entities.Systems.LoreSystem {
[CreateAssetMenu(fileName = "Lore", menuName = "Scriptable Objects/Lore/Lore")]
public class Lore : ScriptableObject {
    public List<Race> races;
}

public static class LoreHolder {
    public static Lore Instance { get; } = Resources.Load<Lore>("Lore");
}
}
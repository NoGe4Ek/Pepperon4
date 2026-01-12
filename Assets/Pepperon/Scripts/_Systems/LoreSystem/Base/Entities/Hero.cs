using UnityEngine;

namespace Pepperon.Scripts.Systems.LoreSystem.Base.Entities {
[CreateAssetMenu(fileName = "Hero", menuName = "Scriptable Objects/Lore/Units/Hero")]
public class Hero : Entity {
    public string heroName;
    public string heroDescription;
    public Sprite icon;
}
}
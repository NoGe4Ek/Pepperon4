using System.Collections.Generic;
using System.Linq;
using Pepperon.Scripts.Entities.Systems.LoreSystem.Base.Abilities;
using Pepperon.Scripts.Entities.Systems.LoreSystem.Base.Upgrades;
using Unity.VisualScripting;
using UnityEngine;

namespace Pepperon.Scripts.Entities.Systems.LoreSystem.Base.Entities {
[CreateAssetMenu(fileName = "Hero", menuName = "Scriptable Objects/Lore/Units/Hero")]
public class Hero : Entity {
    public string heroName;
    public string heroDescription;
    public Sprite icon;
}
}
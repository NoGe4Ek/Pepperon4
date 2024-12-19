using System.Collections.Generic;
using Pepperon.Scripts.Entities.Systems.LoreSystem.Infos;
using Pepperon.Scripts.Entities.Systems.LoreSystem.Upgrades;
using UnityEngine;

namespace Pepperon.Scripts.Entities.Systems.LoreSystem.Entities {
public class Entity : ScriptableObject {
    public GameObject prefab;
    public List<Info> info;
    
    public CommonUpgradeType[] commonUpgradeTypes;
    public Upgrade[] upgrades;
}
}
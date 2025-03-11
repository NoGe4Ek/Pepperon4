using System;
using System.Collections.Generic;
using System.Linq;
using Pepperon.Scripts.Entities.Systems.LoreSystem.Base.Abilities;
using Pepperon.Scripts.Entities.Systems.LoreSystem.Base.Infos;
using Pepperon.Scripts.Entities.Systems.LoreSystem.Base.Upgrades;
using UnityEngine;

namespace Pepperon.Scripts.Entities.Systems.LoreSystem.Base.Entities {
[Serializable]
public enum CommonEntityType {
    MainBuilding,
    Barrack,
    Tower,
    Heroes,
    Units
}

public class Entity : ScriptableObject, IProgressable<EntityProgress> {
    
    
    public GameObject prefab;
    public List<EntityInfo> info;

    public List<CommonUpgradeType> commonUpgradeTypes;
    public List<Upgrade> upgrades;
    public List<Ability> abilities;

    public virtual EntityProgress ToProgress() => new() {
        commonUpgradeTypes = commonUpgradeTypes,
        info = info.Select(i => i.ToProgress()).ToList(),
        upgrades = upgrades.Select(u => u.ToProgress()).ToList(),
        abilities = abilities.Select(a => a.ToProgress()).ToList()
    };
}

[Serializable]
public class EntityProgress : Progress {
    public List<CommonUpgradeType> commonUpgradeTypes;
    [SerializeReference] public List<EntityInfoProgress> info;
    public List<UpgradeProgress> upgrades;
    public List<AbilityProgress> abilities;

    // compatibility progress -> race so
    public int id;

    public EntityProgress WithId(int newId) {
        id = newId;
        return this;
    }
}
}
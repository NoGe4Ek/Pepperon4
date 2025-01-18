using System;
using System.Collections.Generic;
using System.Linq;
using AYellowpaper.SerializedCollections;
using Mirror;
using Pepperon.Scripts.Entities.Systems.LoreSystem.Base.Entities;
using Pepperon.Scripts.Entities.Systems.LoreSystem.Base.Upgrades;
using UnityEngine;
using UnityEngine.Serialization;

namespace Pepperon.Scripts.Entities.Systems.LoreSystem.Base {
[CreateAssetMenu(fileName = "Race", menuName = "Scriptable Objects/Lore/Races/Race")]
public class Race : ScriptableObject, IProgressable<RaceProgress> {
    public string raceName;
    public string raceDescription;
    // public Building mainBuilding;
    // public Building barak;
    // public Building tower;
    // public List<Hero> heroes;
    // public List<Unit> units;

    [SerializedDictionary("Entity type", "Entity list")]
    public SerializedDictionary<CommonEntityType, List<Entity>> entities;

    [SerializedDictionary("Upgrade type", "Upgrade info")]
    public SerializedDictionary<CommonUpgradeType, Upgrade> upgrades;

    public RaceProgress ToProgress() {
        return new RaceProgress {
            // mainBuilding = mainBuilding.ToProgress(),
            // barak = barak.ToProgress(),
            // tower = tower.ToProgress(),
            // heroes = heroes.Select((h, i) => h.ToProgress().WithId(i)).ToList(),
            // units = units.Select((u, i) => u.ToProgress().WithId(i)).ToList(),
            entities = new SerializedDictionary<CommonEntityType, List<EntityProgress>>(entities.ToDictionary(e => e.Key,
                e => e.Value.Select((u, i) => u.ToProgress().WithId(i)).ToList())),
            upgrades = new SerializedDictionary<CommonUpgradeType, UpgradeProgress>(upgrades.ToDictionary(e => e.Key, _ => new UpgradeProgress()))
        };
    }
}

[Serializable]
public class RaceProgress : Progress {
    // public EntityProgress mainBuilding;
    // public EntityProgress barak;
    // public EntityProgress tower;
    // public List<EntityProgress> heroes;
    // public List<EntityProgress> units;
    public SerializedDictionary<CommonEntityType, List<EntityProgress>> entities;
    public SerializedDictionary<CommonUpgradeType, UpgradeProgress> upgrades;
}

[Serializable]
public class EntityId {
    public EntityId() { }

    public EntityId(CommonEntityType entityType, int entityIndex) {
        this.entityType = entityType;
        this.entityIndex = entityIndex;
    }

    public CommonEntityType entityType;
    public int entityIndex;
}

public static class RaceSerializer {
    public static void WriteRace(this NetworkWriter writer, Race race) {
        if (!race) return;
        var index = LoreHolder.Instance.races.IndexOf(race);
        writer.WriteInt(index);
    }

    public static Race WriteRace(this NetworkReader reader) {
        var index = reader.ReadInt();
        return LoreHolder.Instance.races[index];
    }
}
}
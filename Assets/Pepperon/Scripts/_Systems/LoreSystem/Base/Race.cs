using System;
using System.Collections.Generic;
using System.Linq;
using AYellowpaper.SerializedCollections;
using Mirror;
using Pepperon.Scripts.Entities.Systems.LoreSystem.Base;
using Pepperon.Scripts.Entities.Systems.LoreSystem.Base.Upgrades;
using Pepperon.Scripts.Systems.LoreSystem.Base.Cards;
using Pepperon.Scripts.Systems.LoreSystem.Base.Entities;
using UnityEngine;

namespace Pepperon.Scripts.Systems.LoreSystem.Base {
[CreateAssetMenu(fileName = "Race", menuName = "Scriptable Objects/Lore/Races/Race")]
public class Race : ScriptableObject, IProgressable<RaceProgress> {
    public string raceName;
    public string raceDescription;

    [SerializedDictionary("Card rarity", "Card list")]
    public SerializedDictionary<Rarity, List<BaseCard>> cards;
    
    [SerializedDictionary("Entity type", "Entity list")]
    public SerializedDictionary<CommonEntityType, List<Entity>> entities;

    [SerializedDictionary("Upgrade type", "Upgrade info")]
    public SerializedDictionary<CommonUpgradeType, Upgrade> upgrades;

    public RaceProgress ToProgress() {
        return new RaceProgress {
            entities = new SerializedDictionary<CommonEntityType, List<EntityProgress>>(
                entities.ToDictionary(
                    e => e.Key,
                    e => e.Value.Select((entity, i) => entity.WithId(i).ToProgress().WithId(i)).ToList())
            ),
            upgrades = new SerializedDictionary<CommonUpgradeType, UpgradeProgress>(
                upgrades.ToDictionary(e => e.Key, _ => new UpgradeProgress()))
        };
    }
}

[Serializable]
public class RaceProgress : Progress {
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
using System.Collections.Generic;
using System.Linq;
using AYellowpaper.SerializedCollections;
using Mirror;
using Pepperon.Scripts.Entities.Systems.LoreSystem.Entities;
using Pepperon.Scripts.Entities.Systems.LoreSystem.Upgrades;
using Pepperon.Scripts.Utils;
using UnityEngine;

namespace Pepperon.Scripts.Entities.Systems.LoreSystem {
[CreateAssetMenu(fileName = "Race", menuName = "Scriptable Objects/Lore/Races/Race")]
public class Race : ScriptableObject {
    public string raceName;
    public string raceDescription;
    public Building mainBuilding;
    public Building barak;
    public Building tower;
    public List<Hero> heroes;
    public List<Unit> units;
    [SerializedDictionary("Upgrade type", "Upgrade info")]
    public SerializedDictionary<CommonUpgradeType, Upgrade> commonUpgrades;

    public Race DeepCopy() {
        var race = Instantiate(this);
        
        // race.mainBuilding = Instantiate(mainBuilding);
        race.barak = Instantiate(barak);
        // race.tower = Instantiate(tower);
        
        race.heroes = heroes.Select(Instantiate).ToList();
        race.units = units.Select(Instantiate).ToList();
        race.units = units.Select(Instantiate).ToList();

        return race;
    }
}

public static class RaceSerializer {
    public static void WriteRace(this NetworkWriter writer, Race race) {
        Debug.Log("Ser");
        if (!race) return;
        // todo remove last
        
        writer.WriteString(string.Join("", race.name.Split("(").ToList().SkipLast(1))); 
    }

    public static Race ReadRace(this NetworkReader reader) {
        return Resources.Load<Race>(reader.ReadString());
    }
}
}
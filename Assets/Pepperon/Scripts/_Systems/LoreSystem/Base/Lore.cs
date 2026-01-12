using System;
using System.Collections.Generic;
using System.Linq;
using AYellowpaper.SerializedCollections;
using Pepperon.Scripts.Systems.LoreSystem.Base;
using Pepperon.Scripts.Systems.LoreSystem.Base.Cards;
using UnityEngine;

namespace Pepperon.Scripts.Entities.Systems.LoreSystem.Base {
[CreateAssetMenu(fileName = "Lore", menuName = "Scriptable Objects/Lore/Lore")]
public class Lore : ScriptableObject {
    public List<Race> races;

    public SerializedDictionary<Rarity, int> deck;
    [SerializedDictionary("Level", "Rarity to drop chance")]
    public SerializedDictionary<int, RarityDropEntry> dropChances;

    public Dictionary<Rarity, int> GetDeck() {
        return deck.ToDictionary(entry => entry.Key, entry => entry.Value);
    }

    public List<Dictionary<Rarity, int>> GetDropChances() {
        var mapDropChances = new List<Dictionary<Rarity, int>>();
        foreach (var entry in dropChances)
        {
            var rarityDict = entry.Value.dropChances.ToDictionary(
                innerEntry => innerEntry.Key, 
                innerEntry => innerEntry.Value);
            mapDropChances.Add(rarityDict);
        }

        return mapDropChances;
    }
}

public static class LoreHolder {
    public static Lore Instance { get; } = Resources.Load<Lore>("Lore");
}

[Serializable]
public class RarityDropEntry
{
    [SerializedDictionary("Rarity", "Drop chance")]
    public SerializedDictionary<Rarity, int> dropChances;
}
}
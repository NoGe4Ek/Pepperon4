using System;
using System.Collections.Generic;
using System.Linq;
using Pepperon.Scripts.Entities.Systems.LoreSystem.Base;
using Pepperon.Scripts.Systems.LoreSystem.Base;
using Pepperon.Scripts.Systems.LoreSystem.Base.Cards;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Pepperon.Scripts.Managers {
// Менеджер для управления колодой. Ее инициализация, получения карты на основе рассы игрока, обновление
// колоды на основе полученых игроком карт
public class DeckManager : MonoBehaviour {
    public static DeckManager Instance { get; private set; }
    
    public List<Dictionary<Rarity, int>> dropChances;
    public Dictionary<Rarity, int> deck;

    private void Awake() {
        Instance = this;
        dropChances = LoreHolder.Instance.GetDropChances();
        deck = LoreHolder.Instance.GetDeck();
    }

    public CardId GetRandomCardId(int level, Race race) {
        var chances = dropChances[level];
        var filteredChances = chances
            .Where(pair => deck.TryGetValue(pair.Key, out int count) && count > 0)
            .ToDictionary(pair => pair.Key, pair => pair.Value);

        if (chances.Count == 0)
            throw new Exception("No cards in deck");

        var rarity = GetRandomRarity(filteredChances);
        var rarityCards = race.cards[rarity];
        var cardIndex = Random.Range(0, rarityCards.Count);
        
        return new CardId(rarity, cardIndex);
    }

    private static Rarity GetRandomRarity(Dictionary<Rarity, int> chances) {
        int totalWeight = chances.Values.Sum();
        int randomValue = Random.Range(0, totalWeight);
        int current = 0;

        foreach (var pair in chances) {
            current += pair.Value;
            if (randomValue < current) {
                return pair.Key;
            }
        }

        throw new Exception("Failed to select rarity.");
    }
}
}
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Pepperon.Scripts.Systems.LoreSystem.Base.Cards {
public class BaseCard : ScriptableObject {
    public Sprite image;
    public Rarity rarity;
}

[Serializable]
public class CardId {
    public CardId() { }

    public CardId(Rarity rarity, int cardIndex) {
        this.rarity = rarity;
        this.cardIndex = cardIndex;
    }

    public Rarity rarity;
    public int cardIndex;
}

class EffectCard : BaseCard { }

class ArtefactCard : BaseCard {
    public List<ArtefactCard> components = new();
}

public enum Rarity {
    Common, // T1
    Uncommon, // T2
    Rare, // T3
    Epic, // T4
    Legendary, // T5
}
}
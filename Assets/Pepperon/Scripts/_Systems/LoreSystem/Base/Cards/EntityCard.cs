using Pepperon.Scripts.Systems.LoreSystem.Base.Entities;
using UnityEngine;

namespace Pepperon.Scripts.Systems.LoreSystem.Base.Cards {
[CreateAssetMenu(fileName = "EntityCard", menuName = "Scriptable Objects/Lore/Cards/EntityCard")]
class EntityCard : BaseCard {
    public Entity entity;
}
}
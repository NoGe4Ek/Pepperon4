using Pepperon.Scripts.Entities.States._Base;
using UnityEngine;

namespace Pepperon.Scripts.Entities.States.CastingState {
[CreateAssetMenu(fileName = "CastingState", menuName = "Scriptable Objects/Casting State")]
public class Casting : EntityState {
    public const byte Id = 4;
    public override byte GetId => Id;
    public const string TriggerName = "ToCasting"; // todo not needed?
    public override string GetTriggerName => TriggerName;
}

public static class CastingHolder {
    public static EntityState Instance { get; } = Resources.Load<EntityState>("CastingState");
}
}
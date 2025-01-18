using Pepperon.Scripts.Entities.States._Base;
using UnityEngine;

namespace Pepperon.Scripts.Units.States.DyingState {
[CreateAssetMenu(fileName = "DyingState", menuName = "Scriptable Objects/Dying State")]
public class Dying : EntityState {
    public const byte Id = 3;
    public override byte GetId => Id;
    public const string TriggerName = "ToDying";
    public override string GetTriggerName => TriggerName;
}

public class DyingHolder {
    public static EntityState Instance { get; } = Resources.Load<EntityState>("DyingState");
}
}
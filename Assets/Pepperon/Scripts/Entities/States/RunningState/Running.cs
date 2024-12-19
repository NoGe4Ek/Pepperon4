using Pepperon.Scripts.Entities.States._Base;
using UnityEngine;

namespace Pepperon.Scripts.Units.States.RunningState {
[CreateAssetMenu(fileName = "RunningState", menuName = "Scriptable Objects/Running State")]
public sealed class Running : EntityState {
    public const byte Id = 1;
    public override byte GetId => Id;
    public const string TriggerName = "ToRunning";
    public override string GetTriggerName => TriggerName;
}

public class RunningHolder {
    public static EntityState Instance { get; } = Resources.Load<EntityState>("RunningState");
}
}
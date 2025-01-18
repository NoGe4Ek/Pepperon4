using System;
using Pepperon.Scripts.Entities.States._Base;
using UnityEngine;

namespace Pepperon.Scripts.Units.States.IdleState {
[CreateAssetMenu(fileName = "IdleState", menuName = "Scriptable Objects/Idle State")]
public class Idle : EntityState {
    public const byte Id = 0;
    public override byte GetId => Id;
    public const string TriggerName = "ToIdle";
    public override string GetTriggerName => TriggerName;
}

public class IdleHolder {
    public static EntityState Instance { get; } = Resources.Load<EntityState>("IdleState");
}
}
using System;
using Pepperon.Scripts.Entities.States._Base;
using Unity.VisualScripting;
using UnityEngine;

namespace Pepperon.Scripts.Units.States.AttackingState {
[CreateAssetMenu(fileName = "AttackingState", menuName = "Scriptable Objects/Attacking State")]
public class Attacking : EntityState {
    public const byte Id = 2;
    public override byte GetId => Id;
    public const string TriggerName = "ToAttacking";
    public override string GetTriggerName => TriggerName;
}

public static class AttackingStateHolder {
    public static EntityState Instance { get; } = Resources.Load<EntityState>("AttackingState");
}
}
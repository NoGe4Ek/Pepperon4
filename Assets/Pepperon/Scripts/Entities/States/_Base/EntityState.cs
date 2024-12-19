using UnityEngine;

namespace Pepperon.Scripts.Entities.States._Base {
public abstract class EntityState : ScriptableObject {
    public abstract byte GetId { get; }

    public abstract string GetTriggerName { get; }
}
}
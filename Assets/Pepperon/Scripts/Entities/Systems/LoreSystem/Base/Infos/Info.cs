using System;
using UnityEngine;

namespace Pepperon.Scripts.Entities.Systems.LoreSystem.Base.Infos {
public abstract class Info<T> : ScriptableObject, IProgressable<T> where T : Progress {
    public abstract T ToProgress();
}

public abstract class EntityInfo : Info<EntityInfoProgress> { }

[Serializable]
public class EntityInfoProgress : Progress { }
}
using System;
using Pepperon.Scripts.Entities.Systems.LoreSystem.Base;
using UnityEngine;

namespace Pepperon.Scripts.Systems.LoreSystem.Base.Infos {
public class Info<T> : ScriptableObject, IProgressable<T> where T : Progress {
    public virtual T ToProgress() {
        return null;
    }
}

[Serializable]
public class EntityInfoProgress : Progress { }
}
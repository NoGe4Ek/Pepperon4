using System;

namespace Pepperon.Scripts.Entities.Systems.LoreSystem.Base {
public interface IProgressable<out T> where T : Progress {
    T ToProgress();
}

[Serializable]
public abstract class Progress { }
}
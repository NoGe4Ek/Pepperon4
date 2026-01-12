using System;

namespace Pepperon.Scripts.Systems.LoreSystem.Base {
public interface IProgressable<out T> where T : Progress {
    T ToProgress();
}

[Serializable]
public abstract class Progress { }
}
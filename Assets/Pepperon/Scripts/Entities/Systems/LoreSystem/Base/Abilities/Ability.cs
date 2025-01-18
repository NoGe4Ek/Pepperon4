﻿using System;
using UnityEngine;

namespace Pepperon.Scripts.Entities.Systems.LoreSystem.Base.Abilities {

[CreateAssetMenu(fileName = "Ability", menuName = "Scriptable Objects/Lore/Abilities/Ability")]
public class Ability : ScriptableObject, IProgressable<AbilityProgress> {
    public string title;
    public string description;
    public AbilityInfo info;
    public AnimationClip animation;
    public string triggerName;
    
    public string behaviourTypeName;
    public Type BehaviourType
    {
        get => Type.GetType(behaviourTypeName);
        set => behaviourTypeName = value?.AssemblyQualifiedName;
    }

    public AbilityProgress ToProgress() => new() {
        progress = info.ToProgress(),
        behaviourTypeName = behaviourTypeName
    };
}

[Serializable]
public class AbilityProgress : Progress {
    public AbilityInfoProgress progress;
    public string behaviourTypeName;
    public Type BehaviourType
    {
        get => Type.GetType(behaviourTypeName);
        set => behaviourTypeName = value?.AssemblyQualifiedName;
    }
}
}
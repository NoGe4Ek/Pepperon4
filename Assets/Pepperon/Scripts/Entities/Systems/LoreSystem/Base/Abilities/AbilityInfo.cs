using System;
using Pepperon.Scripts.Entities.Systems.LoreSystem.Base.Infos;

namespace Pepperon.Scripts.Entities.Systems.LoreSystem.Base.Abilities {
public abstract class AbilityInfo : Info<AbilityInfoProgress> { }

[Serializable]
public class AbilityInfoProgress : Progress { }
}
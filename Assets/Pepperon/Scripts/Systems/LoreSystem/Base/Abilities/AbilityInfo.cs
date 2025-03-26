using System;
using Pepperon.Scripts.Entities.Systems.LoreSystem.Base;
using Pepperon.Scripts.Systems.LoreSystem.Base.Infos;

namespace Pepperon.Scripts.Systems.LoreSystem.Base.Abilities {
public class AbilityInfo : Info<AbilityInfoProgress> { }

[Serializable]
public class AbilityInfoProgress : Progress { }
}
using System;
using Pepperon.Scripts.Systems.LoreSystem.Base.Infos.EntityInfos;
using UnityEngine;

namespace Pepperon.Scripts.Systems.LoreSystem.Base.Infos {
[CreateAssetMenu(fileName = "Rotation", menuName = "Scriptable Objects/Lore/Info/Rotation")]
public class RotationInfo : EntityInfo {
    public float rotationSpeed = 5f;

    public override EntityInfoProgress ToProgress() {
        return new RotationInfoProgress() {
            rotationSpeed = rotationSpeed
        };
    }
}

[Serializable]
public class RotationInfoProgress : EntityInfoProgress {
    public float rotationSpeed;
}
}
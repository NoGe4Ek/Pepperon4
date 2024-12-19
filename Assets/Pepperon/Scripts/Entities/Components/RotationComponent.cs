using System;
using System.Linq;
using Pepperon.Scripts.AI.Units.ScriptableObjects;
using Pepperon.Scripts.Entities.Controllers;
using Pepperon.Scripts.Entities.Systems.LoreSystem;
using Pepperon.Scripts.Entities.Systems.LoreSystem.Infos;
using Pepperon.Scripts.ScriptableObjects;
using UnityEngine;
using UnityEngine.Serialization;

namespace Pepperon.Scripts.Units.Components {
public class RotationComponent : MonoBehaviour {
    private RotationInfo rotationInfo;
    public Vector3 RotationDirection { private get; set; }

    private void Awake() {
        rotationInfo = GetComponent<EntityController>().entity.info.OfType<RotationInfo>().First();
    }

    private void Update() {
        RotateToPointer(RotationDirection);
    }

    private void RotateToPointer(Vector3 lookDirection) {
        if (!(lookDirection.magnitude > 0.001f)) return;
        
        lookDirection.y = 0;
        var targetRotation = Quaternion.LookRotation(lookDirection);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationInfo.rotationSpeed * Time.deltaTime);
    }
}
}
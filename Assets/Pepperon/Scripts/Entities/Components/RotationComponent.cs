using System.Linq;
using Pepperon.Scripts.Entities.Controllers;
using Pepperon.Scripts.Systems.LoreSystem.Base.Infos;
using UnityEngine;

namespace Pepperon.Scripts.Units.Components {
    public class RotationComponent : MonoBehaviour {
        private RotationInfo rotationInfo;
        private RotationInfoProgress rotationInfoProgress;
        public Vector3 RotationDirection { private get; set; }

        private void Start() {
            rotationInfo = GetComponent<EntityController>().entity.info.OfType<RotationInfo>().First();
            rotationInfoProgress = GetComponent<EntityController>().entityProgress.info.OfType<RotationInfoProgress>()
                .First();
        }

        private void Update() {
            RotateToPointer(RotationDirection);
        }

        private void RotateToPointer(Vector3 lookDirection) {
            if (!(lookDirection.magnitude > 0.001f)) return;

            lookDirection.y = 0;
            var targetRotation = Quaternion.LookRotation(lookDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation,
                rotationInfoProgress.rotationSpeed * Time.deltaTime);
        }
    }
}
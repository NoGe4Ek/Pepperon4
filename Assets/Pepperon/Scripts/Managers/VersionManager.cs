using System;
using TMPro;
using UnityEngine;

namespace Pepperon.Scripts.Managers {
    
    public class VersionManager: MonoBehaviour {
        public const string Version = "0.0.1";
        [SerializeField] public TMP_Text versionText;

        private void Awake() {
            versionText.text = Version;
        }
    }
}
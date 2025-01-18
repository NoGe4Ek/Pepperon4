using UnityEngine;

namespace Pepperon.Scripts.AI.Units.ScriptableObjects {
[CreateAssetMenu(fileName = "ProjectSettings", menuName = "Project Settings", order = 1)]
public class ProjectSettingsSO : ScriptableObject {
    public float unitDetectionDelay;
    public float aiUpdateDelay;
}
}
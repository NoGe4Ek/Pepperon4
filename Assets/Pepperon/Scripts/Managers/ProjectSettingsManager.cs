using Pepperon.Scripts.AI.Units.ScriptableObjects;
using UnityEngine;

namespace Pepperon.Scripts.Managers {
public class ProjectSettingsManager : MonoBehaviour {
    public ProjectSettingsSO projectSettings;

    // Паттерн Singleton
    private static ProjectSettingsManager _instance;

    public static ProjectSettingsSO Instance {
        get {
            if (_instance == null) {
                _instance = FindObjectOfType<ProjectSettingsManager>();

                if (_instance == null) {
                    GameObject singletonObject = new GameObject();
                    _instance = singletonObject.AddComponent<ProjectSettingsManager>();
                    singletonObject.name = typeof(ProjectSettingsManager) + " (Singleton)";
                    DontDestroyOnLoad(singletonObject);
                }
            }

            return _instance.projectSettings;
        }
    }

    void Awake() {
        // Убедитесь, что есть только один экземпляр SettingsManager
        if (_instance == null) {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else {
            Destroy(gameObject);
        }
    }
}
}
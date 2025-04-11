using UnityEngine;

namespace Pepperon.Scripts.DI {
public class G : MonoBehaviour {
    [SerializeField] public Camera mainCamera;
    [SerializeField] public Camera minimapCamera;
    [SerializeField] public Canvas minimapCanvas;
    
    public static G Instance { get; private set; }

    private void Awake() {
        Instance = this;
    }
}
}
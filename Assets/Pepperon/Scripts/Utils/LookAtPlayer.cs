using UnityEngine;

namespace Pepperon.Scripts.Utils {
public class LookAtPlayer : MonoBehaviour {
    public Transform cam;

    private void Awake() {
        if (Camera.main != null) cam = Camera.main.transform;
    }

    void LateUpdate() {
        //transform.rotation = Quaternion.identity;
        transform.LookAt(transform.position + cam.forward);
    }
}
}
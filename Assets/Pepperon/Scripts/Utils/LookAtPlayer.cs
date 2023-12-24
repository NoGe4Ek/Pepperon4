using UnityEngine;
using UnityEngine.Events;

namespace Pepperon.Scripts.Utils {
public class LookAtPlayer : MonoBehaviour {
    public Transform cam;
    
    void LateUpdate() {
        //transform.rotation = Quaternion.identity;
        transform.LookAt(transform.position + cam.forward);
    }
}
}
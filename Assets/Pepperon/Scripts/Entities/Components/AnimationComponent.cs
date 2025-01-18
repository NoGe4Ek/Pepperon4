using Mirror;
using UnityEngine;

namespace Pepperon.Scripts.Units.Components {
// todo NetworkAnimator works, but only for current clients. Next clients lose info about current triggers
public class AnimationComponent : MonoBehaviour {
    [SerializeField] public Animator animator;
    // [SerializeField] private NetworkAnimator networkAnimator;

    public float NormalizedTime() => animator.GetCurrentAnimatorStateInfo(0).normalizedTime;
    
    public void SetTrigger(string triggerName) {
        animator.SetTrigger(triggerName);
    }

    public void ResetTrigger(string triggerName) {
        animator.ResetTrigger(triggerName);
    }

    public void SetFloat(int id, float value) {
        animator.SetFloat(id, value);
    }
}
}
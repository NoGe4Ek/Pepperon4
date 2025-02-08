using UnityEngine;

namespace Pepperon.Scripts.UI {
public abstract class BaseScreen : MonoBehaviour {
    protected Canvas canvas;

    public virtual void Awake() {
        Hide();
        canvas = GetComponentInParent<Canvas>();
    }

    public void Show() {
        gameObject.SetActive(true);
        enabled = true;
    }

    public void Hide() {
        gameObject.SetActive(false);
        enabled = false;
    }
    
    public virtual void Initialize(object args) { }
}
}
using System;
using Pepperon.Scripts.Entities.Components;
using Pepperon.Scripts.Units.Components;
using UnityEngine;

namespace Pepperon.Scripts.Managers {
public class BattleManager : MonoBehaviour {
    public static BattleManager Instance { get; private set; }

    public static event Action<GameObject, GameObject> OnKill;

    private void Awake() {
        Instance = this;
    }

    public static void ApplyDamage(GameObject from, GameObject to, float damage) {
        var healthComponent = to.GetComponent<HealthComponent>();
        if (healthComponent && !healthComponent.isDied) {
            if (healthComponent.GetCurrentHealth() - damage < 1)
                OnKill?.Invoke(from, to);
            var newHealth = healthComponent.TakeDamage(damage);

            // Debug.Log(healthComponent.transform.name + " get damage from " + from.name);
        }
    }
}
}
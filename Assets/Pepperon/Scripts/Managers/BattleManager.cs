using System;
using Pepperon.Scripts.Entities.Components;
using Pepperon.Scripts.Entities.Controllers;
using Pepperon.Scripts.Systems.LoreSystem.Base.Entities;
using Pepperon.Scripts.Units.Components;
using UnityEngine;

namespace Pepperon.Scripts.Managers {
public class BattleManager : MonoBehaviour {
    public static BattleManager Instance { get; private set; }

    public static event Action<GameObject, GameObject> OnKill;

    private void Awake() {
        Instance = this;
    }

    private void Start() {
        OnKill += CheckHeroDied;
    }

    public static void ApplyDamage(GameObject from, GameObject to, float damage) {
        var healthComponent = to.GetComponent<HealthComponent>();
        if (!healthComponent || healthComponent.isDied) return;
        
        if (healthComponent.GetCurrentHealth() - damage < 1)
            OnKill?.Invoke(from, to);
        var newHealth = healthComponent.TakeDamage(damage);
    }

    private void CheckHeroDied(GameObject killerObject, GameObject diedObject) {
        if (!diedObject.TryGetComponent(out EntityController entityController)) return;
        var hero = entityController.entity as Hero;
        if (hero == null) return;
        SessionManager.Instance.knownPlayers[entityController.playerType].heroAvailability[hero] = true;
    }
}
}
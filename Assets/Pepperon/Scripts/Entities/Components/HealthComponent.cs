using System;
using System.Linq;
using Mirror;
using Pepperon.Scripts.Entities.Controllers;
using Pepperon.Scripts.Systems.LoreSystem.Base.Infos;
using UnityEngine.UI;

namespace Pepperon.Scripts.Entities.Components {
public class HealthComponent : NetworkBehaviour {
    private SurvivabilityInfo survivabilityInfo;
    private SurvivabilityInfoProgress survivabilityInfoProgress;
    [SyncVar] private float currentHealth;
    public bool isDied;
    public Slider slider;

    private void Start() {
        survivabilityInfo = GetComponent<EntityController>().entity.info.OfType<SurvivabilityInfo>().First();
        survivabilityInfoProgress = GetComponent<EntityController>().entityProgress.info.OfType<SurvivabilityInfoProgress>().First();
        currentHealth = survivabilityInfoProgress.maxHealth;
    }

    private void Update() {
        slider.value = currentHealth / survivabilityInfoProgress.maxHealth;
    }

    public float GetCurrentHealth() => currentHealth;

    public float TakeDamage(float damage) {
        currentHealth -= damage;
        if (currentHealth < 1)
            isDied = true;
        return currentHealth;
    }
}
}
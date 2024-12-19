using System.Linq;
using Mirror;
using Pepperon.Scripts.Entities.Controllers;
using Pepperon.Scripts.Entities.Systems.LoreSystem.Infos;
using UnityEngine.UI;

namespace Pepperon.Scripts.Entities.Components {
public class HealthComponent : NetworkBehaviour {
    private SurvivabilityInfo survivabilityInfo;
    [SyncVar] private float currentHealth;
    public bool isDied;
    public Slider slider;

    private void Awake() {
        survivabilityInfo = GetComponent<EntityController>().entity.info.OfType<SurvivabilityInfo>().First();
        currentHealth = survivabilityInfo.maxHealth;
    }

    private void Update() {
        slider.value = currentHealth / survivabilityInfo.maxHealth;
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
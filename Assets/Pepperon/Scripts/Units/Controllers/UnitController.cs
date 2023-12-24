using System;
using Pepperon.Scripts.AI.Units.ScriptableObjects;
using Pepperon.Scripts.AI.Units.States;
using Pepperon.Scripts.Units.Managers;
using Pepperon.Scripts.Utils;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Pepperon.Scripts.AI.Units.Controllers {
public class UnitController: MonoBehaviour {
    [SerializeField] public UnitSO unit;
    [SerializeField] private StateManager stateManager;
    
    [SerializeField] private Transform obstacleRange;
    [SerializeField] private Transform chaseRange;
    [SerializeField] private Transform attackRange;

    public float currentHealth;
    public Slider slider;

    private void Awake() {
        currentHealth = unit.maxHealth;
    }

    private void Update() {
        // test (than to awake)
        obstacleRange.localScale = new Vector3(unit.obstacleRange, unit.obstacleRange, unit.obstacleRange);
        chaseRange.localScale = new Vector3(unit.chaseRange, unit.chaseRange, unit.chaseRange);
        attackRange.localScale = new Vector3(unit.attackRange, unit.attackRange, unit.attackRange);

        slider.value = currentHealth / unit.maxHealth;
        
        if (currentHealth < 1) {
            stateManager.SwitchState(new Dying());
        }
    }
}
}
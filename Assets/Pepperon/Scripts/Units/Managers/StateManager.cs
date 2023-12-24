using System.Collections;
using System.Collections.Generic;
using Pepperon.Scripts.AI;
using Pepperon.Scripts.AI.Units.Controllers;
using Pepperon.Scripts.AI.Units.ScriptableObjects;
using Pepperon.Scripts.AI.Units.States;
using UnityEngine;
using UnityEngine.Events;

namespace Pepperon.Scripts.Units.Managers {
public class StateManager : MonoBehaviour {
    [SerializeField] public MovementData movementData;
    [SerializeField] public AttackingData attackingData;
    
    [SerializeField] public Animator animator;
    [SerializeField] public List<SteeringBehaviour> behaviours;
    [SerializeField] public Vector3 movementInput;
    [SerializeField] public ContextSolver movementDirectionSolver;
    
    [SerializeField] public UnityEvent<Vector3> onMovementInput;
    [SerializeField] public UnityEvent<Vector3> onPointerInput;

    public UnitSO unit;
    private State state;

    private void Awake() {
        unit = GetComponent<UnitController>().unit;
        // animator.SetFloat(unit.runningAnimationSpeedName, unit.speed);
        // animator.SetFloat(unit.attackingAnimationSpeedName, unit.attackDelay);
    }

    private void Start() {
        state = new Idle();
        state.EnterState(this);
    }

    private void Update() {
        // TEST (than to awake in controller)
        animator.SetFloat(unit.runningAnimationSpeedName, unit.speed);
        animator.SetFloat(unit.attackingAnimationSpeedName, unit.AttackAnimationMultiplier);
        
        

        state.Update(this);
    }

    public void SwitchState(State newState) {
        state.ExitState(this);
        animator.ResetTrigger(state.TriggerName);
        state = newState;
        state.EnterState(this);
    }

    public void WaitAndSwitchState(State newState) {
        StartCoroutine(Wait(newState));
    }

    private IEnumerator Wait(State newState) {
        yield return new WaitForSeconds(3);
        SwitchState(newState);
    }
}
}
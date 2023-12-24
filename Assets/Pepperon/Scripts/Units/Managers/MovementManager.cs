using Pepperon.Scripts.AI.Units.Controllers;
using Pepperon.Scripts.AI.Units.ScriptableObjects;
using UnityEngine;

public class MovementManager : MonoBehaviour {
    private Rigidbody rb2d;
    private UnitSO unit;
    private Vector3 oldMovementInput;
    private float currentSpeed;

    // Should be public with setter
    public Vector3 MovementInput { get; set; }

    private void Awake() {
        rb2d = GetComponent<Rigidbody>();
        unit = GetComponent<UnitController>().unit;
    }

    private void FixedUpdate() {
        Move();
    }

    private void Move() {
        if (MovementInput.magnitude > 0 && currentSpeed >= 0) {
            oldMovementInput = MovementInput;
            currentSpeed += unit.acceleration * unit.speed * Time.deltaTime;
        }
        else {
            currentSpeed -= unit.deacceleration * unit.speed * Time.deltaTime;
        }

        currentSpeed = Mathf.Clamp(currentSpeed, 0, unit.speed);
        rb2d.velocity = oldMovementInput * currentSpeed;
    }
}
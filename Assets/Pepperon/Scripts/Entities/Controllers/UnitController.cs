using Pepperon.Scripts.Units.Components;
using Pepperon.Scripts.Units.Components.StateMachines;
using UnityEngine;

namespace Pepperon.Scripts.Entities.Controllers {
public class UnitController : EntityController {
    [SerializeField] public MovementComponent movementComponent;
    [SerializeField] public StateMachineComponent stateMachineComponent;
}
}
using UnityEngine;

namespace Pepperon.Scripts.Entities.Systems.LoreSystem.Infos {
[CreateAssetMenu(fileName = "Movement", menuName = "Scriptable Objects/Lore/Info/Movement")]
public class MovementInfo : Info {
    public readonly int runningAnimationSpeedName = Animator.StringToHash("runningAnimationSpeed");
    public float speed = 3f;
    public float acceleration = 50f;
    public float deacceleration = 100f;

    public float agentColliderSize = 0.5f; // take obstacle force
    public float obstacleRange = 4f;
    public float chaseRange = 30f;
    public float targetReachedThreshold = 0.5f;
}
}
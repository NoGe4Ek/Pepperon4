using System;
using UnityEngine;

namespace Pepperon.Scripts.Entities.Systems.LoreSystem.Base.Infos {
[CreateAssetMenu(fileName = "Movement", menuName = "Scriptable Objects/Lore/Info/Movement")]
public class MovementInfo : EntityInfo {
    public readonly int runningAnimationSpeedName = Animator.StringToHash("runningAnimationSpeed");
    public float speed = 3f;
    public float acceleration = 50f;
    public float deacceleration = 100f;

    public float agentColliderSize = 0.5f; // take obstacle force
    public float obstacleRange = 4f;
    public float chaseRange = 30f;
    public float targetReachedThreshold = 0.5f;

    public override EntityInfoProgress ToProgress() => new MovementInfoProgress() {
        speed = speed,
        acceleration = acceleration,
        deacceleration = deacceleration,
        agentColliderSize = agentColliderSize,
        obstacleRange = obstacleRange,
        chaseRange = chaseRange,
        targetReachedThreshold = targetReachedThreshold
    };
}

[Serializable]
public class MovementInfoProgress : EntityInfoProgress {
    public float speed;
    public float acceleration;
    public float deacceleration;

    public float agentColliderSize; // take obstacle force
    public float obstacleRange;
    public float chaseRange;
    public float targetReachedThreshold;
}
}
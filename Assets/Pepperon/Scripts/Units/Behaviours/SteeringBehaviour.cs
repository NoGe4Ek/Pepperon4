using System.Collections.Generic;
using UnityEngine;

public abstract class SteeringBehaviour : MonoBehaviour {
    public abstract (float[] danger, float[] interest)
        GetSteering(float[] danger, float[] interest);
}

public static class Directions {
    public static readonly List<Vector3> eightDirections = new() {
        new Vector3(0, 0, 1).normalized,
        new Vector3(1, 0, 1).normalized,
        new Vector3(1, 0, 0).normalized,
        new Vector3(1, 0, -1).normalized,
        new Vector3(0, 0, -1).normalized,
        new Vector3(-1, 0, -1).normalized,
        new Vector3(-1, 0, 0).normalized,
        new Vector3(-1, 0, 1).normalized
    };
}
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Pepperon.Scripts.Entities.MovementSystem.Behaviours {
public abstract class BaseSteeringBehaviour : MonoBehaviour {
    public abstract (float[] danger, float[] interest)
        GetSteering(float[] danger, float[] interest);
}

public static class Directions {
    public const int DirectionCount = 16;

    private static readonly Lazy<List<Vector3>> LazyDirections = new(() => GenerateDirections(DirectionCount));
    public static readonly List<Vector3> directions = LazyDirections.Value;

    private static List<Vector3> GenerateDirections(int count) {
        List<Vector3> generated = new List<Vector3>();
        float angleIncrement = 360f / count;
        for (int i = 0; i < count; i++) {
            float angle = -i * angleIncrement; // minus, cause clockwise
            float radians = Mathf.Deg2Rad * angle;
            generated.Add(new Vector3(Mathf.Cos(radians), 0, Mathf.Sin(radians)).normalized);
        }

        return generated;
    }
}
}
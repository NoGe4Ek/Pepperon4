using System;
using System.Collections;
using System.Collections.Generic;
using Pepperon.Scripts.AI;
using Pepperon.Scripts.AI.Units.ScriptableObjects;
using Pepperon.Scripts.Entities.MovementSystem.Behaviours;
using Pepperon.Scripts.Entities.Systems.LoreSystem;
using Pepperon.Scripts.Entities.Systems.LoreSystem.Base.Infos;
using Pepperon.Scripts.ScriptableObjects;
using Pepperon.Scripts.Units.Components;
using Pepperon.Scripts.Units.Data;
using UnityEngine;

public class ObstacleAvoidanceBehaviour : BaseSteeringBehaviour
{
    private MovementData movementData;
    private MovementInfo movementInfo;

    private void Awake() {
        movementData = GetComponentInParent<MovementData>();
    }

    private void Start() {
        movementInfo = GetComponentInParent<MovementComponent>().movementInfo;
    }

    public override (float[] danger, float[] interest) GetSteering(float[] danger, float[] interest)
    {
        foreach (Collider obstacleCollider in movementData.obstacles)
        {
            Vector3 directionToObstacle
                = obstacleCollider.ClosestPoint(transform.position) - transform.position;
            float distanceToObstacle = directionToObstacle.magnitude;

            //calculate weight based on the distance Enemy<--->Obstacle
            float weight
                = distanceToObstacle <= movementInfo.agentColliderSize
                    ? 1
                    : (movementInfo.obstacleRange - distanceToObstacle) / movementInfo.obstacleRange;

            Vector3 directionToObstacleNormalized = directionToObstacle.normalized;

            //Add obstacle parameters to the danger array
            for (int i = 0; i < Directions.DirectionCount; i++)
            {
                float result = Vector3.Dot(directionToObstacleNormalized, Directions.directions[i]);

                float valueToPutIn = result * weight;

                //override value only if it is higher than the current one stored in the danger array
                if (valueToPutIn > danger[i])
                {
                    danger[i] = valueToPutIn;
                }
            }
        }
        
        return (danger, interest);
    }
}
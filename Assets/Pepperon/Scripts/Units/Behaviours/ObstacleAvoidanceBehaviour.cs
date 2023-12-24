using System.Collections;
using System.Collections.Generic;
using Pepperon.Scripts.AI;
using Pepperon.Scripts.AI.Units.Controllers;
using Pepperon.Scripts.AI.Units.ScriptableObjects;
using UnityEngine;

public class ObstacleAvoidanceBehaviour : SteeringBehaviour
{
    [SerializeField] private bool showGizmo = true;

    //gizmo parameters
    float[] dangersResultTemp = null;
    
    private MovementData movementData;
    private UnitSO unit;

    private void Awake() {
        movementData = GetComponentInParent<MovementData>();
        unit = GetComponentInParent<UnitController>().unit;
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
                = distanceToObstacle <= unit.agentColliderSize
                    ? 1
                    : (unit.obstacleRange - distanceToObstacle) / unit.obstacleRange;

            Vector3 directionToObstacleNormalized = directionToObstacle.normalized;

            //Add obstacle parameters to the danger array
            for (int i = 0; i < Directions.eightDirections.Count; i++)
            {
                float result = Vector3.Dot(directionToObstacleNormalized, Directions.eightDirections[i]);

                float valueToPutIn = result * weight;

                //override value only if it is higher than the current one stored in the danger array
                if (valueToPutIn > danger[i])
                {
                    danger[i] = valueToPutIn;
                }
            }
        }

        dangersResultTemp = danger;
        return (danger, interest);
    }

    private void OnDrawGizmos()
    {
        if (showGizmo == false)
            return;

        if (Application.isPlaying && dangersResultTemp != null)
        {
            if (dangersResultTemp != null)
            {
                Gizmos.color = Color.red;
                for (int i = 0; i < dangersResultTemp.Length; i++)
                {
                    Gizmos.DrawRay(
                        transform.position,
                        Directions.eightDirections[i] * dangersResultTemp[i] * 2
                    );
                }
            }
        }
    }
}
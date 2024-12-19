using Mirror;
using Pepperon.Scripts.Managers;
using UnityEngine;

namespace Pepperon.Scripts.Units.Components {
public class ProjectileComponent : NetworkBehaviour {
    private Vector3 lastTargetPosition;
    private Vector3 currentTargetPosition;
    private Transform target;
    private float speed;
    public float arcHeight;
    private float damage;

    Vector3 _startPosition;
    float _progress;
    private float distance;

    public void Init(Transform initTarget, float projectileSpeed, float projectileDamage) {
        target = initTarget;
        currentTargetPosition = initTarget.position;
        speed = projectileSpeed;
        damage = projectileDamage;
        arcHeight = 1;

        _startPosition = transform.position;
        distance = Vector3.Distance(_startPosition, currentTargetPosition);

        Vector3 direction = currentTargetPosition - _startPosition;
        Quaternion lookRotation = Quaternion.LookRotation(direction);
        transform.rotation = lookRotation;
    }

    private void Update() {
        if (!isServer) return;
        lastTargetPosition = currentTargetPosition;
        currentTargetPosition = target == null ? lastTargetPosition : target.position;

        _progress = Mathf.Min(_progress + Time.deltaTime * (speed / distance), 1.0f);

        Vector3 currentTargetPos = currentTargetPosition;
        distance = Vector3.Distance(_startPosition, currentTargetPos);

        Vector3 nextPos = Vector3.Lerp(_startPosition, currentTargetPos, _progress);
        float parabola = 1.0f - 4.0f * (_progress - 0.5f) * (_progress - 0.5f);
        nextPos.y += parabola * arcHeight;

        Vector3 direction = currentTargetPos - transform.position;
        direction.y = -30;

        if (direction.sqrMagnitude > 0.1f) {
            Quaternion lookRotation = Quaternion.LookRotation(direction);

            float angle = Mathf.Lerp(-30, 30, _progress);
            Quaternion tiltRotation = Quaternion.Euler(angle, lookRotation.eulerAngles.y, 0);

            transform.rotation = Quaternion.Slerp(transform.rotation, tiltRotation, Time.deltaTime * speed);
        }

        transform.position = nextPos;

        if (Vector3.Distance(transform.position, currentTargetPos) < 0.1f) {
            OnHitTarget();
        }

        // RpcSyncMovement(transform.position);
        if (currentTargetPosition.y < -30) Destroy(gameObject);
    }

    private void OnHitTarget() {
        if (target != null)
            BattleManager.ApplyDamage(gameObject, target.gameObject, damage);
        Destroy(gameObject);
    }

    // [ClientRpc]
    // private void RpcSyncMovement(Vector3 position) {
    //     transform.position = position;
    // }
}
}
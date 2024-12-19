using System.Collections.Generic;
using Pepperon.Scripts.Utils;
using UnityEngine;

// todo on release change lists with hash set
namespace Pepperon.Scripts.Units.Data {
public class AttackingData : MonoBehaviour {
    [SerializeField] public List<Transform> targets = new();
    [SerializeField] public Transform currentTarget;
    public Task currentAttackCoroutine;
    public enum AttackStateEnum {
        Attacking,
        WaitingForAttack
    }
}
}
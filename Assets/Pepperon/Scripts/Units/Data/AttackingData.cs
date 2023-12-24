using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Pepperon.Scripts.AI {
public class AttackingData : MonoBehaviour {
    public List<Transform> targets;
    public Transform currentTarget;

    private void Update() {
        if (targets.Count > 0) {
            currentTarget = targets.FirstOrDefault();
        }
    }
}
}
using System.Collections.Generic;
using UnityEngine;

namespace Pepperon.Scripts.Entities.Components {
public class SpawnComponent : MonoBehaviour {
    [SerializeField] public List<Transform> unitSpawnPoints;
}
}
using Mirror;
using UnityEngine;

namespace Pepperon.Scripts.Managers {
public class UpgradeManager : NetworkBehaviour {
    [SerializeField] public GameObject meleePrefab;

    [ClientRpc]
    private void RpcUpgradeMeleeDamage() {
        // meleePrefab.GetComponent<BaseAttackingComponent>().attackingHolder.attack += 10;
        // Debug.Log("Upgrade melee unit");
    }
}
}
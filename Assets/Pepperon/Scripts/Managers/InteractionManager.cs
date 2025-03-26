using Mirror;
using Pepperon.Scripts.Controllers;
using Pepperon.Scripts.UI.Interaction;
using UnityEngine;

namespace Pepperon.Scripts.Managers {
public class InteractionManager : NetworkBehaviour {
    public static InteractionManager Instance { get; private set; }

    [SerializeField] private LayerMask interactionLayer;
    
    public int selectedHeroIndex;
    public bool isChooseBarrackMode;
    public Camera currentCamera;

    private void Awake() {
        Instance = this;
    }

    private void Update() {
        if (!isChooseBarrackMode) return;
        if (!Input.GetMouseButtonDown(0)) return;
        var ray = currentCamera.ScreenPointToRay(Input.mousePosition);

        if (!Physics.Raycast(ray, out var hit, Mathf.Infinity, interactionLayer)) return;
        if (hit.collider.GetComponent<BlinkInteraction>() == null) {
            ExitChooseBarrackMode();
            Debug.Log("Exit from no BlinkInteraction");
        }
    }

    public void EnterChooseBarrackMode(int heroIndex) {
        isChooseBarrackMode = true;
        selectedHeroIndex = heroIndex;
        foreach (var barrack in PlayerController.localPlayer.barracks) {
            barrack.GetComponent<BlinkInteraction>().Enable();
        }
    }

    public void OnBarrackChosen(int barrackIndex) {
        SpawnManager.Instance.SpawnHero(PlayerController.localPlayer.playerId, selectedHeroIndex, barrackIndex);
        ExitChooseBarrackMode();
        Debug.Log("OnBarrackChosen");
    }

    public void ExitChooseBarrackMode() {
        isChooseBarrackMode = false;
        foreach (var barrack in PlayerController.localPlayer.barracks) {
            barrack.GetComponent<BlinkInteraction>().Disable();
        }
    }
}
}
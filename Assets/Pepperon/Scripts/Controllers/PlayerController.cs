using System;
using System.Collections.Generic;
using System.Linq;
using Mirror;
using Pepperon.Scripts.Entities.Systems.LoreSystem.Base;
using Pepperon.Scripts.Managers;
using Pepperon.Scripts.Systems.LoreSystem.Base;
using Pepperon.Scripts.Systems.LoreSystem.Base.Entities;
using Pepperon.Scripts.UI;
using Pepperon.Scripts.Units.Components.StateMachines;
using UnityEngine;
using Race = Pepperon.Scripts.Systems.LoreSystem.Base.Race;

namespace Pepperon.Scripts.Controllers {
public class PlayerController : NetworkBehaviour {
    public GameObject mainBuilding;
    public readonly SyncList<GameObject> barracks = new();
    public readonly SyncDictionary<Hero, GameObject> heroes = new();

    public bool IsPlayerDefeat(GameObject diedObject) {
        if (mainBuilding != diedObject && IsBuildingStillActive(mainBuilding))
            return false;

        return barracks.All(barrack => barrack == diedObject || !IsBuildingStillActive(barrack));
    }

    private bool IsBuildingStillActive(GameObject building) {
        if (building == null) return false;


        if (!building.TryGetComponent(out StateMachineComponent stateMachine)) return false;
        if (stateMachine.IsDying()) return false;


        Debug.Log("CheckPlayerDefeat: Building " + building.name + " is alive");
        return true;
    }

    [SyncVar(hook = nameof(OnGoldChange))] public int gold;

    private void OnGoldChange(int oldGold, int newGold) {
        OnNewGold?.Invoke(newGold);
    }

    [SerializeField] public RaceProgress progress;

    public event Action<int, string> OnNewMessage;
    public event Action OnRaceChanged;

    [ClientRpc]
    public void SendAlert(int targetPlayerId, string message) {
        OnNewMessage?.Invoke(targetPlayerId, message);
    }

    public event Action<int> OnNewGold;
    public static event Action OnLocalPlayerReady;

    [SyncVar(hook = nameof(OnRaceChange))] public Race race;

    private void SetRace(Race newRace) {
        if (newRace == LoreHolder.Instance.races[1]) return;
        race = newRace;
        progress = newRace.ToProgress();
    }

    private void OnRaceChange(Race oldRace, Race newRace) {
        if (newRace == LoreHolder.Instance.races[1]) return;
        progress = newRace.ToProgress();
        OnRaceChanged?.Invoke();
    }

    [SyncVar(hook = nameof(OnPlayerIdChange))]
    public int playerId;

    [SyncVar] public UserResponse user;

    // [Client]
    public void OnPlayerIdChange(int oldPlayerId, int newPlayerId) {
        if (newPlayerId == -1) return;
        SessionManager.Instance.AddKnownPlayer(newPlayerId);
    }

    public void Awake() {
        playerId = -1;
        race = LoreHolder.Instance.races[1];
    }

    public static PlayerController localPlayer;

    public override void OnStartLocalPlayer() {
        base.OnStartLocalPlayer();
        localPlayer = this;
        CmdAuthenticate(PlayerPrefs.GetString("PlayerId"));
        OnLocalPlayerReady?.Invoke();
    }

    public bool isAuthenticated;

    [Command]
    private void CmdAuthenticate(string id) {
        var player = BootManager.Instance.Lobby.players.First(player => player.user.id == id);
        user = player.user;
        isAuthenticated = true;
        
        switch (player.race) {
            case UI.Race.NONE:
                break;
            case UI.Race.HUMANITY:
                SetRace(LoreHolder.Instance.races[0]);
                break;
            case UI.Race.ORC:
                SetRace(LoreHolder.Instance.races[2]);
                break;
        }

        if (BootManager.Instance.Lobby.players.Count == SessionManager.Instance.players.Count
            && SessionManager.Instance.players.Values.All(p => p.isAuthenticated))
            SessionManager.Instance.state = GameState.Setup;
    }
}
}
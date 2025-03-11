using System;
using System.Collections.Generic;
using System.Linq;
using Mirror;
using Pepperon.Scripts.Entities.Systems.LoreSystem.Base;
using Pepperon.Scripts.Managers;
using Pepperon.Scripts.UI;
using UnityEngine;
using Race = Pepperon.Scripts.Entities.Systems.LoreSystem.Base.Race;

namespace Pepperon.Scripts.Controllers {
public class PlayerController : NetworkBehaviour {
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

    public void OnPlayerIdChange(int oldPlayerId, int newPlayerId) {
        if (newPlayerId == -1) return;
        SessionManager.Instance.AddKnownPlayer(newPlayerId);
    }

    public void Awake() {
        playerId = -1;
        race = LoreHolder.Instance.races[1];
    }

    public GameObject mainBuilding;
    public List<GameObject> barracks;

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
        switch (player.race) {
            case UI.Race.NONE:
                break;
            case UI.Race.HUMANITY:
                SetRace(LoreHolder.Instance.races[0]);
                break;
            case UI.Race.ORC:
                SetRace(LoreHolder.Instance.races[2]);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        user = player.user;
        isAuthenticated = true;

        if (BootManager.Instance.Lobby.players.Count == SessionManager.Instance.players.Count
            && SessionManager.Instance.players.Values.All(p => p.isAuthenticated))
            SessionManager.Instance.state = true;
    }
}
}
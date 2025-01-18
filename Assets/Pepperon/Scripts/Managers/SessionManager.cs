using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Mirror;
using Pepperon.Scripts.Controllers;
using Pepperon.Scripts.Entities.Controllers;
using Pepperon.Scripts.Entities.Systems.LoreSystem.Base;
using Pepperon.Scripts.Entities.Systems.LoreSystem.Base.Entities;
using UnityEngine;
using UnityEngine.SceneManagement;
using Task = Pepperon.Scripts.Utils.Task;

namespace Pepperon.Scripts.Managers {
public class SessionManager : NetworkBehaviour {
    public static SessionManager Instance { get; private set; }
    [SerializeField] public List<Transform> mainBuildingStartPoints;
    public bool isGameNow = false;
    public static event Action OnGameEnd;
    public static event Action OnGameStart;
    private Task startGameDelayCoroutine;

    public readonly Dictionary<NetworkConnectionToClient, PlayerController> players = new();
    public readonly Dictionary<int, PlayerController> knownPlayers = new();
    public void AddKnownPlayer(int playerId) {
        var playerController = FindObjectsOfType<PlayerController>()
            .FirstOrDefault(player => player.playerId == playerId);
        knownPlayers[playerId] = playerController;
    }

    private void Awake() {
        Instance = this;
        isGameNow = false;
    }

    private void Start() {
        if (!isServer) return;
        BattleManager.OnKill += OnDie;
        startGameDelayCoroutine = new Task(StartGame());
    }

    private void Update() {
        if (!isServer) return;
        if (NetworkManager.singleton.maxConnections == Instance.players.Count && !isGameNow) {
            startGameDelayCoroutine.Stop();
            SetupGame();
        }
    }

    private IEnumerator StartGame() {
        yield return new WaitForSeconds(5);
        SetupGame();
    }

    private void SetupGame() {
        for (var i = 0; i < players.Values.Count; i++) {
            var player = players.Values.ToList()[i];

            player.SetRace(LoreHolder.Instance.races.First());
            player.SetPlayerId(i);
            
            var startPos = mainBuildingStartPoints[i];
            Vector3 directionToLook = Vector3.zero - startPos.position;
            Quaternion lookRotation = Quaternion.LookRotation(directionToLook);
            var mainBuilding = Instantiate(player.race.entities[CommonEntityType.Barak].First().prefab, startPos.position, lookRotation);
            var buildingController = mainBuilding.GetComponent<BuildingController>();
            buildingController.playerType = player.playerId;
            player.mainBuilding = mainBuilding;

            NetworkServer.Spawn(mainBuilding, player.connectionToClient);
            
            buildingController.entityId = new EntityId(CommonEntityType.Barak, 0);
        }

        OnGameStart?.Invoke();
        isGameNow = true;
    }

    private void SetupUnits() { }

    private void OnDie(GameObject killerObject, GameObject diedObject) {
        if (players.Any(it => it.Value.mainBuilding == diedObject)) {
            Debug.Log(diedObject.name + " LOSE");
            OnGameEnd?.Invoke();
            isGameNow = false;
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
        }
    }
}
}
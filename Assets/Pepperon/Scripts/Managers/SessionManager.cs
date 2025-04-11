using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Mirror;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Pepperon.Scripts.Controllers;
using Pepperon.Scripts.Entities.Components;
using Pepperon.Scripts.Entities.Controllers;
using Pepperon.Scripts.Entities.Systems.LoreSystem.Base;
using Pepperon.Scripts.Networking.Services;
using Pepperon.Scripts.Systems.LoreSystem.Base;
using Pepperon.Scripts.Systems.LoreSystem.Base.Entities;
using Pepperon.Scripts.UI;
using Unity.VisualScripting;
using UnityEngine;
using Task = Pepperon.Scripts.Utils.Task;

namespace Pepperon.Scripts.Managers {
[Serializable]
public enum GameState {
    WaitingForPlayers,
    Setup,
    InProgress,
    Paused,
    Finished
}

public class SessionManager : NetworkBehaviour {
    public static SessionManager Instance { get; private set; }

    [SerializeField] private List<Transform> mainBuildingStartPoints;
    [SerializeField] private List<Transform> pathPoints;
    private Task gameTimerCoroutine;
    private float gameTime;

    // List of players (server only).
    // NetworkConnectionToClient available only on server 
    public readonly Dictionary<NetworkConnectionToClient, PlayerController> players = new();

    // List of players (client and server).
    // Store PlayerController for each integer id (order number of player connection)
    // P.S. Known on client side
    public readonly Dictionary<int, PlayerController> knownPlayers = new();
 
    public static event Action<string> OnTimeTick;


    [SyncVar(hook = nameof(OnGameTimerChanged))]
    public string gameTimer;

    private void OnGameTimerChanged(string oldGameTimer, string newGameTimer) {
        OnTimeTick?.Invoke(newGameTimer);
    }

    [SyncVar(hook = nameof(OnStateChange))]
    public GameState state;

    private void OnStateChange(GameState oldState, GameState newState) {
        switch (newState) {
            case GameState.WaitingForPlayers:
                break;
            case GameState.Setup:
                break;
            case GameState.InProgress:
                break;
            case GameState.Paused:
                break;
            case GameState.Finished:
                break;
        }
    }

    // [Server]
    public void AddPlayer(NetworkConnectionToClient conn, PlayerController player) {
        // Save player connection to controller (for server needs)
        players.Add(conn, player);
        
        // Save player id to controller (for client needs)
        // Trigger AddKnownPlayer on clients
        var playerId = players.Count - 1;
        player.playerId = playerId;
        knownPlayers[playerId] = player;
    }

    // [Server]
    public void RemovePlayer(NetworkConnectionToClient conn) {
        var player = players[conn];
        knownPlayers.Remove(player.playerId);
        RpcRemoveKnownPlayer(player.playerId);

        players.Remove(conn);
        if (players.Count( it => !new Regex(@"^bot-\d+$").IsMatch(it.Value.user.id)) == 0)
            MatchService.EndMatch(
                new MatchResult(
                    MatchResultType.ABNORMAL,
                    DateTimeOffset.UtcNow,
                    DateTimeOffset.UtcNow,
                    TimeSpan.FromSeconds(gameTime),
                    "",
                    new Dictionary<string, PlayerStatistic>()
                )
            );
    }

    [ClientRpc]
    private void RpcRemoveKnownPlayer(int playerId) {
        // todo
    }

    // Find all Controllers on client scene and get one with player's id
    // [Client]
    public void AddKnownPlayer(int playerId) {
        var playerController =
            FindObjectsOfType<PlayerController>().FirstOrDefault(player => player.playerId == playerId);
        knownPlayers[playerId] = playerController;
    }

    private void Awake() {
        Instance = this;
        state = GameState.WaitingForPlayers;
    }

    private void Start() {
        if (!isServer) return;
        BattleManager.OnKill += CheckPlayerDefeat;
    }

    private void Update() {
        if (!isServer) return;
        if (state == GameState.Setup) {
            SetupGame();
        }

        if (state == GameState.InProgress && (gameTimerCoroutine == null || !gameTimerCoroutine.Running)) {
            gameTimerCoroutine = new Task(UpdateGameTimer());
        }
    }

    private IEnumerator UpdateGameTimer() {
        float lastUpdateTime = Time.time;

        while (state == GameState.InProgress) {
            float currentTime = Time.time;
            gameTime += currentTime - lastUpdateTime;
            lastUpdateTime = currentTime;

            int minutes = Mathf.FloorToInt(gameTime / 60);
            int seconds = Mathf.FloorToInt(gameTime % 60);

            gameTimer = $"{minutes:00}:{seconds:00}";

            yield return new WaitForSeconds(0.5f);
        }
    }

    private void SetupGame() {
        Debug.Log("SetupGame: players - " + players.Values.Count);
        for (var i = 0; i < players.Values.Count; i++) {
            var player = players.Values.ToList()[i];

            var startPos = mainBuildingStartPoints[i];

            Vector3 directionToLook = Vector3.zero - startPos.position;
            Quaternion lookRotation = Quaternion.LookRotation(directionToLook);

            var mainBuilding = Instantiate(player.race.entities[CommonEntityType.MainBuilding].First().prefab,
                startPos.position, lookRotation);
            var mainBuildingController = mainBuilding.GetComponent<BuildingController>();

            Vector3 centerBarrackPosition = startPos.position + directionToLook.normalized * 5f;
            var centerBarrack = Instantiate(player.race.entities[CommonEntityType.Barrack][0].prefab,
                centerBarrackPosition, lookRotation);
            var centerBarrackController = centerBarrack.GetComponent<BuildingController>();
            centerBarrack.GetComponent<SpawnComponent>().path = new List<Transform> {
                mainBuildingStartPoints[(i + players.Count / 2) % players.Count]
            };

            Quaternion leftBarrackRotation = lookRotation * Quaternion.Euler(0, -90, 0);
            Vector3 leftBarrackDirection = leftBarrackRotation * Vector3.forward;
            Vector3 leftBarrackPosition = startPos.position + leftBarrackDirection * 5f;
            var leftBarrack = Instantiate(player.race.entities[CommonEntityType.Barrack][1].prefab, leftBarrackPosition,
                leftBarrackRotation);
            var leftBarrackController = leftBarrack.GetComponent<BuildingController>();
            var leftBarrackPath = GetPath(startPos);
            leftBarrack.GetComponent<SpawnComponent>().path = leftBarrackPath;

            var rightBarrackRotation =
                lookRotation * Quaternion.Euler(0, 90, 0);
            Vector3 rightBarrackDirection = rightBarrackRotation * Vector3.forward;
            Vector3 rightBarrackPosition = startPos.position + rightBarrackDirection * 5f;
            var rightBarrack = Instantiate(player.race.entities[CommonEntityType.Barrack][2].prefab,
                rightBarrackPosition, rightBarrackRotation);
            var rightBarrackController = rightBarrack.GetComponent<BuildingController>();
            var rightBarrackPath = GetPath(startPos);
            rightBarrackPath.Reverse();
            rightBarrack.GetComponent<SpawnComponent>().path = rightBarrackPath;

            mainBuildingController.playerType = player.playerId;
            centerBarrackController.playerType = player.playerId;
            leftBarrackController.playerType = player.playerId;
            rightBarrackController.playerType = player.playerId;

            NetworkServer.Spawn(mainBuilding, player.connectionToClient);
            NetworkServer.Spawn(centerBarrack, player.connectionToClient);
            NetworkServer.Spawn(leftBarrack, player.connectionToClient);
            NetworkServer.Spawn(rightBarrack, player.connectionToClient);
            
            player.mainBuilding = mainBuilding;
            player.barracks.Add(centerBarrack);
            player.barracks.Add(leftBarrack);
            player.barracks.Add(rightBarrack);

            mainBuildingController.entityId = new EntityId(CommonEntityType.MainBuilding, 0);
            centerBarrackController.entityId = new EntityId(CommonEntityType.Barrack, 0);
            leftBarrackController.entityId = new EntityId(CommonEntityType.Barrack, 1);
            rightBarrackController.entityId = new EntityId(CommonEntityType.Barrack, 2);

            for (var heroIndex = 0; heroIndex < player.race.entities[CommonEntityType.Heroes].Count; heroIndex++) {
                player.heroes[heroIndex] = null;
            }
        }

        state = GameState.InProgress;
    }
    
    private List<Transform> GetPath(Transform startPoint) {
        var startIndex = pathPoints.IndexOf(startPoint);
        var reorderedList = pathPoints.GetRange(startIndex, pathPoints.Count - startIndex);
        reorderedList.RemoveAt(0);
        reorderedList.AddRange(pathPoints.GetRange(0, startIndex));
        return reorderedList;
    }

    private void CheckPlayerDefeat(GameObject killerObject, GameObject diedObject) {
        if (!diedObject.TryGetComponent(out BuildingController buildingController)) return;
        if (players.Count(it => !it.Value.IsPlayerDefeat(diedObject)) != 1) return;
        
        state = GameState.Finished;
        MatchService.EndMatch(
            new MatchResult(
                MatchResultType.NORMAL,
                DateTimeOffset.UtcNow,
                DateTimeOffset.UtcNow,
                TimeSpan.FromSeconds(gameTime),
                players
                    .First(it => !it.Value.IsPlayerDefeat(diedObject)).Value.user.id,
                new Dictionary<string, PlayerStatistic>()
            )
        );
    }
}
}
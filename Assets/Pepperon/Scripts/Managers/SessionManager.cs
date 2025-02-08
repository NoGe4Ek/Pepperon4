using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Mirror;
using Pepperon.Scripts.Controllers;
using Pepperon.Scripts.Entities.Components;
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
    [SerializeField] public List<Transform> pathPoints;

    [SyncVar(hook = nameof(OnStateChange))]
    public bool state;

    private void OnStateChange(bool oldState, bool newState) {
        if (newState)
            OnGameStart?.Invoke();
        else
            OnGameEnd?.Invoke();
    }

    public bool isGameNow = false;
    public static event Action OnGameEnd;
    public static event Action OnGameStart;
    public static event Action OnGameConfigured;

    public static event Action<PlayerController> OnNewPlayer;
    public static event Action<int> OnNewKnownPlayer;
    public static event Action<int> OnRemoveKnownPlayer;
    
    public readonly Dictionary<NetworkConnectionToClient, PlayerController> players = new();
    public readonly Dictionary<int, PlayerController> knownPlayers = new();
    [SyncVar] public int playersCount;
    
    public void AddPlayer(NetworkConnectionToClient conn, PlayerController player) {
        // Save player connection to controller (on server)
        players.Add(conn, player);
        // Save player id to controller (on clients)
        player.playerId = players.Count - 1;
        AddKnownPlayer(players.Count - 1);

        playersCount++;
        OnNewPlayer?.Invoke(player);
    }

    public void RemovePlayer(NetworkConnectionToClient conn) {
        var player = players[conn];
        knownPlayers.Remove(player.playerId);
        OnRemoveKnownPlayer?.Invoke(player.playerId);
        RpcRemoveKnownPlayer(player.playerId);
        
        players.Remove(conn);
        
        Debug.Log("Remove player, id = " + player.playerId);
    }

    [ClientRpc]
    private void RpcRemoveKnownPlayer(int playerId) {
        OnRemoveKnownPlayer?.Invoke(playerId);
    }

    public void AddKnownPlayer(int playerId) {
        var playerController = FindObjectsOfType<PlayerController>()
            .FirstOrDefault(player => player.playerId == playerId);
        knownPlayers[playerId] = playerController;
        OnNewKnownPlayer?.Invoke(playerId);

        Debug.Log("AddKnownPlayer " + playerId);
    }

    private void Awake() {
        Instance = this;
        isGameNow = false;
    }

    private void Start() {
        if (!isServer) return;
        BattleManager.OnKill += OnPlayerDefeat;
    }

    private void Update() {
        if (!isServer) return;
        if (state && !isGameNow) {
            SetupGame();
        }
    }

    private void SetupGame() {
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
                mainBuildingStartPoints[(i + 1) % players.Count]
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

            player.mainBuilding = mainBuilding;
            player.barracks.Add(centerBarrack);
            player.barracks.Add(leftBarrack);
            player.barracks.Add(rightBarrack);

            NetworkServer.Spawn(mainBuilding, player.connectionToClient);
            NetworkServer.Spawn(centerBarrack, player.connectionToClient);
            NetworkServer.Spawn(leftBarrack, player.connectionToClient);
            NetworkServer.Spawn(rightBarrack, player.connectionToClient);

            mainBuildingController.entityId = new EntityId(CommonEntityType.MainBuilding, 0);
            centerBarrackController.entityId = new EntityId(CommonEntityType.Barrack, 0);
            leftBarrackController.entityId = new EntityId(CommonEntityType.Barrack, 1);
            rightBarrackController.entityId = new EntityId(CommonEntityType.Barrack, 2);
        }

        OnGameConfigured?.Invoke();
        isGameNow = true;
    }

    private List<Transform> GetPath(Transform startPoint) {
        var startIndex = pathPoints.IndexOf(startPoint);
        var reorderedList = pathPoints.GetRange(startIndex, pathPoints.Count - startIndex);
        reorderedList.RemoveAt(0);
        reorderedList.AddRange(pathPoints.GetRange(0, startIndex));
        return reorderedList;
    }

    private void SetupUnits() { }

    private void OnPlayerDefeat(GameObject killerObject, GameObject diedObject) {
        if (players.Any(it => it.Value.mainBuilding == diedObject)) {
            Debug.Log(diedObject.name + " LOSE");
            state = false;
            isGameNow = false;
            // PlayerController.localPlayer.SendAlert(diedObject.GetComponent<EntityController>().playerType, "You was defeated");
            // SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
        }
    }
}
}
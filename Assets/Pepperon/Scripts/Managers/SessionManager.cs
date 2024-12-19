using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Mirror;
using Pepperon.Scripts.Controllers;
using Pepperon.Scripts.Entities.Controllers;
using Pepperon.Scripts.Entities.Systems.LoreSystem;
using Unity.VisualScripting;
using UnityEditor.VersionControl;
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

            player.race = LoreHolder.Instance.races.First().DeepCopy();
            
            player.playerType = i;
            var startPos = mainBuildingStartPoints[i];
            Vector3 directionToLook = Vector3.zero - startPos.position;
            Quaternion lookRotation = Quaternion.LookRotation(directionToLook);
            var mainBuilding = Instantiate(player.race.barak.prefab, startPos.position, lookRotation);
            var buildingController = mainBuilding.GetComponent<BuildingController>();
            buildingController.entity = player.race.barak;
            buildingController.playerType = player.playerType;
            NetworkServer.Spawn(mainBuilding, player.connectionToClient);

            player.mainBuilding = mainBuilding;
        }
        
        OnGameStart?.Invoke();
        isGameNow = true;
    }

    private void SetupUnits() {
        
    }

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
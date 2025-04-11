using System.Collections;
using System.Linq;
using Mirror;
using Pepperon.Scripts.Entities.Components;
using Pepperon.Scripts.Entities.Controllers;
using Pepperon.Scripts.Entities.Systems.LoreSystem.Base;
using Pepperon.Scripts.Systems.LoreSystem.Base;
using Pepperon.Scripts.Systems.LoreSystem.Base.Entities;
using Pepperon.Scripts.Utils;
using UnityEngine;

namespace Pepperon.Scripts.Managers {
public class SpawnManager : NetworkBehaviour {
    public static SpawnManager Instance { get; private set; }

    private Task spawnCoroutine;

    private void Awake() {
        Instance = this;
    }

    private void Update() {
        if (!isServer) return;

        if (SessionManager.Instance.state == GameState.InProgress &&
            (spawnCoroutine == null || !spawnCoroutine.Running)) {
            spawnCoroutine = new Task(WaitAndSpawnUnit());
        }
    }

    private IEnumerator WaitAndSpawnUnit() {
        SpawnUnit();
        yield return new WaitForSeconds(5);
    }

    private void SpawnUnit() {
        for (var playerIndex = 0; playerIndex < SessionManager.Instance.players.Count; playerIndex++) {
            var player = SessionManager.Instance.players.Values.ToList()[playerIndex];

            for (var unitIndex = 0; unitIndex < player.race.entities[CommonEntityType.Units].Count; unitIndex++) {
                var unit = player.race.entities[CommonEntityType.Units][unitIndex];
                
                for (var barrackIndex = 0; barrackIndex < player.race.entities[CommonEntityType.Barrack].Count; barrackIndex++) {
                    var barrack = player.barracks[barrackIndex];
                    if(barrack == null) continue;
                    
                    var barrackSpawnComponent = barrack.GetComponent<SpawnComponent>();
                    var point = barrackSpawnComponent.GetRandomPointInRegion();

                    GameObject unitInstance = Instantiate(unit.prefab, point, Quaternion.identity);
                    var unitController = unitInstance.GetComponentInParent<UnitController>();
                    unitController.movementComponent.movementData.points.AddRange(barrackSpawnComponent.path);
                    unitController.playerType = player.playerId;
                    unitController.entityId = new EntityId(CommonEntityType.Units, unitIndex);

                    NetworkServer.Spawn(unitInstance, player.connectionToClient);
                }
            }
        }
    }

    [Command(requiresAuthority = false)]
    public void SpawnHero(int playerId, int heroIndex, int selectedBarrackIndex) {
        var player = SessionManager.Instance.knownPlayers[playerId];
        if (player.gold < 500) {
            player.SendAlert(playerId, "Not enough gold for spawn hero. Need: 500");
            return;
        }

        var selectedBarrack = player.barracks[selectedBarrackIndex];
        var barrackSpawnComponent = selectedBarrack.GetComponent<SpawnComponent>();
        var point = barrackSpawnComponent.GetRandomPointInRegion();

        Hero hero = (player.race.entities[CommonEntityType.Heroes][heroIndex] as Hero)!;
        Debug.Log("SpawnHero: hero - " + player.heroes[heroIndex]);
        if (player.heroes[heroIndex] != null) return;
        
        GameObject heroInstance = Instantiate(hero.prefab, point, Quaternion.identity);
        var unitController = heroInstance.GetComponentInParent<UnitController>();
        unitController.playerType = player.playerId;
        unitController.movementComponent.movementData.points.AddRange(barrackSpawnComponent.path);

        NetworkServer.Spawn(heroInstance, player.connectionToClient);
        player.heroes[heroIndex] = heroInstance;
        
        unitController.entityId = new EntityId(CommonEntityType.Heroes, heroIndex);

        player.gold -= 500;
    }
}
}
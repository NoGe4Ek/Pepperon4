using System.Collections;
using System.Linq;
using Mirror;
using Pepperon.Scripts.Controllers;
using Pepperon.Scripts.Entities.Components;
using Pepperon.Scripts.Entities.Controllers;
using Pepperon.Scripts.Entities.Systems.LoreSystem.Base;
using Pepperon.Scripts.Entities.Systems.LoreSystem.Base.Entities;
using Pepperon.Scripts.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace Pepperon.Scripts.Managers {
public class SpawnManager : NetworkBehaviour {
    public static SpawnManager Instance { get; private set; }

    private Task spawnCoroutine;

    private void Awake() {
        Instance = this;
    }

    private void Update() {
        if (!isServer) return;

        if (SessionManager.Instance.isGameNow && (spawnCoroutine == null || !spawnCoroutine.Running)) {
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
                for (var barrackIndex = 0;
                     barrackIndex < player.race.entities[CommonEntityType.Barrack].Count;
                     barrackIndex++) {
                    var barrack = player.barracks[barrackIndex];
                    var barrackSpawnComponent = barrack.GetComponent<SpawnComponent>();
                    var point = barrackSpawnComponent.GetRandomPointInRegion();

                    GameObject unitInstance = Instantiate(unit.prefab, point, Quaternion.identity);
                    var unitController = unitInstance.GetComponentInParent<UnitController>();
                    unitController.movementComponent.movementData.points.AddRange(barrackSpawnComponent.path);
                    unitController.playerType = player.playerId;

                    NetworkServer.Spawn(unitInstance, player.connectionToClient);

                    unitController.entityId = new EntityId(CommonEntityType.Units, unitIndex);
                }
            }
        }
    }

    [Command(requiresAuthority = false)]
    public void SpawnHero(int playerId) {
        var player = SessionManager.Instance.knownPlayers[playerId];
        if (player.gold < 500) {
            player.SendAlert(playerId, "Not enough gold for spawn hero. Need: 500");
            return;
        }

        for (var index = 0; index < player.race.entities[CommonEntityType.Heroes].Count; index++) {
            var hero = player.race.entities[CommonEntityType.Heroes][index];
            var centerBarrack = player.barracks[0];
            var barrackSpawnComponent = centerBarrack.GetComponent<SpawnComponent>();
            var point = barrackSpawnComponent.GetRandomPointInRegion();

            GameObject heroInstance = Instantiate(hero.prefab, point, Quaternion.identity);
            var unitController = heroInstance.GetComponentInParent<UnitController>();
            unitController.playerType = player.playerId;
            unitController.movementComponent.movementData.points.AddRange(barrackSpawnComponent.path);

            NetworkServer.Spawn(heroInstance, player.connectionToClient);

            unitController.entityId = new EntityId(CommonEntityType.Heroes, index);

            player.gold -= 500;
        }
    }
}
}
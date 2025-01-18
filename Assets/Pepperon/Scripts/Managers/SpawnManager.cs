using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Mirror;
using Pepperon.Scripts.Entities.Components;
using Pepperon.Scripts.Entities.Controllers;
using Pepperon.Scripts.Entities.Systems.LoreSystem;
using Pepperon.Scripts.Entities.Systems.LoreSystem.Base;
using Pepperon.Scripts.Entities.Systems.LoreSystem.Base.Entities;
using Pepperon.Scripts.Utils;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Pepperon.Scripts.Managers {
public class SpawnManager : NetworkBehaviour {
    private Task spawnCoroutine;
    private Task heroSpawnCoroutine;

    private void Update() {
        if (!isServer) return;

        if (SessionManager.Instance.isGameNow && (spawnCoroutine == null || !spawnCoroutine.Running)) {
            spawnCoroutine = new Task(WaitAndSpawnUnit());
        }
        if (SessionManager.Instance.isGameNow && (heroSpawnCoroutine == null || !heroSpawnCoroutine.Running))
            heroSpawnCoroutine = new Task(WaitAndSpawnHero());
    }


    private IEnumerator WaitAndSpawnUnit() {
        SpawnUnit();
        yield return new WaitForSeconds(5); 
    }
    
    private IEnumerator WaitAndSpawnHero() {
        yield return new WaitForSeconds(5);
        SpawnHero();
        yield return new WaitForSeconds(5000);
    }

    private void SpawnUnit() {
        for (var i = 0; i < SessionManager.Instance.players.Count; i++) {
            var player = SessionManager.Instance.players.Values.ToList()[i];
            var points = player.mainBuilding.GetComponent<SpawnComponent>().unitSpawnPoints;

            for (var index = 0; index < player.race.entities[CommonEntityType.Units].Count; index++) {
                var unit = player.race.entities[CommonEntityType.Units][index];
                var point = points[Random.Range(0, points.Count)];

                GameObject unitInstance = Instantiate(unit.prefab, point.position, point.rotation);
                var unitController = unitInstance.GetComponentInParent<UnitController>();
                unitController.playerType = player.playerId;
                var targetPoint =
                    SessionManager.Instance.mainBuildingStartPoints[(i + 1) % SessionManager.Instance.players.Count];
                unitController.movementComponent.movementData.points.Add(targetPoint);
                
                NetworkServer.Spawn(unitInstance, player.connectionToClient);
                
                unitController.entityId = new EntityId(CommonEntityType.Units, index);
            }
        }

        // Image[] images = unitSpawn.GetComponentsInChildren<Image>();
        // Image[] rangeImages = rangeUnitSpawn.GetComponentsInChildren<Image>();
        // Image specificImage = images.FirstOrDefault(image => image.name == "Fill");
        // Image rangeSpecificImage = rangeImages.FirstOrDefault(image => image.name == "Fill");
        // if (specificImage)
        //     specificImage.color = connectionToClient.connectionId == 0 ? Color.green : Color.red;
        // if (rangeSpecificImage)
        //     rangeSpecificImage.color = connectionToClient.connectionId == 0 ? Color.green : Color.red;
    }
    
    private void SpawnHero() {
        for (var i = 0; i < SessionManager.Instance.players.Count - 1; i++) {
            var player = SessionManager.Instance.players.Values.ToList()[i];
            var points = player.mainBuilding.GetComponent<SpawnComponent>().unitSpawnPoints;

            for (var index = 0; index < player.race.entities[CommonEntityType.Heroes].Count; index++) {
                var hero = player.race.entities[CommonEntityType.Heroes][index];
                var point = points[Random.Range(0, points.Count)];

                GameObject heroInstance = Instantiate(hero.prefab, point.position, point.rotation);
                var unitController = heroInstance.GetComponentInParent<UnitController>();
                unitController.playerType = player.playerId;
                var targetPoint =
                    SessionManager.Instance.mainBuildingStartPoints[(i + 1) % SessionManager.Instance.players.Count];
                unitController.movementComponent.movementData.points.Add(targetPoint);
                
                NetworkServer.Spawn(heroInstance, player.connectionToClient);
                
                unitController.entityId = new EntityId(CommonEntityType.Heroes, index);
            }
        }
    }
}
}
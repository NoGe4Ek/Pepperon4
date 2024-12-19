using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Mirror;
using Pepperon.Scripts.Entities.Components;
using Pepperon.Scripts.Entities.Controllers;
using Pepperon.Scripts.Entities.Systems.LoreSystem;
using Pepperon.Scripts.Entities.Systems.LoreSystem.Entities;
using Pepperon.Scripts.Utils;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Pepperon.Scripts.Managers {
public class SpawnManager : NetworkBehaviour {
    private Task spawnCoroutine;

    private void Update() {
        if (!isServer) return;

        if (SessionManager.Instance.isGameNow && (spawnCoroutine == null || !spawnCoroutine.Running))
            spawnCoroutine = new Task(WaitAndSpawnUnit());
    }


    private IEnumerator WaitAndSpawnUnit() {
        yield return new WaitForSeconds(3);
        SpawnUnit();
    }

    private void SpawnUnit() {
        for (var i = 0; i < SessionManager.Instance.players.Count; i++) {
            var player = SessionManager.Instance.players.Values.ToList()[i];
            var points = player.mainBuilding.GetComponent<SpawnComponent>().unitSpawnPoints;
            
            foreach (var unit in player.race.units) {
                var point = points[Random.Range(0, points.Count)];

                GameObject unitInstance = Instantiate(unit.prefab, point.position, point.rotation);
                NetworkServer.Spawn(unitInstance, player.connectionToClient);

                var unitController = unitInstance.GetComponentInParent<UnitController>();
                unitController.entity = unit;
                unitController.playerType = player.playerType;
                var targetPoint =
                    SessionManager.Instance.mainBuildingStartPoints[(i + 1) % SessionManager.Instance.players.Count];
                unitController.movementComponent.movementData.points.Add(targetPoint);
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
}
}
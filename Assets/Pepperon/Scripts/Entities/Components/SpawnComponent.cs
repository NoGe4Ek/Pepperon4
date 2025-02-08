using System.Collections.Generic;
using UnityEngine;

namespace Pepperon.Scripts.Entities.Components {
public class SpawnComponent : MonoBehaviour {
    [SerializeField] private Transform spawnRegion;
    [SerializeField] private Transform building;
    public List<Transform> path;

    public Vector3 GetRandomPointInRegion() {
        // Получаем размеры spawnRegion
        Mesh spawnRegionMesh = spawnRegion.GetComponent<MeshFilter>().sharedMesh;
        Vector3 spawnRegionSize = spawnRegionMesh.bounds.size;
        Vector3 spawnRegionScale = spawnRegion.localScale;
        Vector3 spawnRegionCenter = spawnRegion.position;

        // Учитываем размеры и масштаб плоскости
        Vector3 planeSize = new Vector3(
            spawnRegionSize.x * spawnRegionScale.x,
            0,
            spawnRegionSize.z * spawnRegionScale.z
        );

        // Получаем центр и радиус круга (building)
        Mesh buildingMesh = building.GetComponent<MeshFilter>().sharedMesh;
        Vector3 buildingScale = building.localScale;
        float buildingRadius = buildingMesh.bounds.extents.x * buildingScale.x; // Радиус круга
        Vector3 buildingCenter = building.position;

        // Генерация случайной точки
        float randomAngle = Random.Range(0, Mathf.PI * 2); // Случайный угол
        float maxRadius = Mathf.Min(planeSize.x, planeSize.z) / 2f; // Половина минимального размера плоскости
        float randomRadius = Mathf.Sqrt(Random.Range(buildingRadius * buildingRadius, maxRadius * maxRadius));

        // Преобразуем полярные координаты в декартовые
        Vector3 offset = new Vector3(
            Mathf.Cos(randomAngle) * randomRadius,
            0,
            Mathf.Sin(randomAngle) * randomRadius
        );

        // Смещаем относительно центра spawnRegion
        Vector3 randomPoint = buildingCenter + offset;
        randomPoint.y = 0;

        // Убедимся, что точка находится в пределах плоскости
        randomPoint.x = Mathf.Clamp(randomPoint.x, spawnRegionCenter.x - planeSize.x / 2, spawnRegionCenter.x + planeSize.x / 2);
        randomPoint.z = Mathf.Clamp(randomPoint.z, spawnRegionCenter.z - planeSize.z / 2, spawnRegionCenter.z + planeSize.z / 2);

        return randomPoint;
    }
}
}
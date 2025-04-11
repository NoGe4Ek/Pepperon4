using System.Linq;
using Pepperon.Scripts.DI;
using Pepperon.Scripts.Entities.Components;
using Pepperon.Scripts.Entities.Controllers;
using Pepperon.Scripts.Systems.LoreSystem.Base.Infos;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Pepperon.Scripts.Managers {
public class SelectionManager : MonoBehaviour {
    [SerializeField] private LayerMask selectionLayerMask;
    [SerializeField] private GameObject selectionGroup;
    [SerializeField] private TMP_Text entityHealthCount;
    [SerializeField] private Image entityHealthImage;
    private EntityController selectedEntityController;

    private void Update() {
        if (Input.GetMouseButtonDown(0)) {
            var ray = G.Instance.mainCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out var hit, Mathf.Infinity, selectionLayerMask)) {
                EntityController entityController = hit.collider.GetComponent<EntityController>();
                SelectEntity(entityController);
            }
            else {
                DeselectEntity();
            }
        }

        if (selectedEntityController != null) {
            entityHealthCount.text =
                selectedEntityController.GetComponent<HealthComponent>().GetCurrentHealth()
                + "/"
                + selectedEntityController.entityProgress().info.OfType<SurvivabilityInfoProgress>().First().maxHealth;

            entityHealthImage.fillAmount = selectedEntityController.GetComponent<HealthComponent>().GetCurrentHealth() /
                                           selectedEntityController.entity.info.OfType<SurvivabilityInfo>().First().maxHealth;
        }
        else {
            DeselectEntity();
        }
    }

    private void SelectEntity(EntityController entityController) {
        selectionGroup.SetActive(true);
        selectedEntityController = entityController;
    }

    private void DeselectEntity() {
        selectionGroup.SetActive(false);
        selectedEntityController = null;
    }

    public static SelectionManager Instance { get; private set; }

    private void Awake() {
        Instance = this;
    }
}
}
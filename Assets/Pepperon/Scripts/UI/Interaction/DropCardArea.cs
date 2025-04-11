using Pepperon.Scripts.Controllers;
using Pepperon.Scripts.Entities.Controllers;
using UnityEngine;

namespace Pepperon.Scripts.UI.Interaction {
public interface IDropCardArea {
    void OnDropCard(CardInteraction card);
}

public class BuildingCardDropArea : MonoBehaviour, IDropCardArea {
    public void OnDropCard(CardInteraction card) {
        // PlayerController.localPlayer;
    }
}
}
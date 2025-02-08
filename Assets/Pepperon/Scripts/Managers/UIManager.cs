using System;
using System.Collections;
using Mirror;
using Pepperon.Scripts.Controllers;
using Pepperon.Scripts.Entities.Systems.LoreSystem.Base.Upgrades;
using Pepperon.Scripts.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Pepperon.Scripts.Managers {
public class UIManager : NetworkBehaviour {
    public Button meleeUpgradeDamageButton;
    public Button rangeUpgradeDamageButton;
    public Button survivabilityUpgradeDamageButton;
    public Button heroButton;
    public TMP_Text goldTextView;
    public TMP_Text alertTextView;
    public Task messageCoroutine;

    private void Awake() {
        PlayerController.OnLocalPlayerReady += () => {
            PlayerController.localPlayer.OnNewGold += gold => goldTextView.text = "Gold: " + gold;
            PlayerController.localPlayer.OnNewMessage += OnMessageReceived;
        };
    }

    private void Start() {
        meleeUpgradeDamageButton.onClick.AddListener(OnMeleeUpgradeDamageClick);
        rangeUpgradeDamageButton.onClick.AddListener(OnRangeUpgradeDamageClick);
        survivabilityUpgradeDamageButton.onClick.AddListener(OnUpgradeHealthClick);
        heroButton.onClick.AddListener(OnHeroClick);
    }

    private void OnMessageReceived(int playerId, string message) {
        if (PlayerController.localPlayer.playerId != playerId) return;
        
        messageCoroutine?.Stop();
        messageCoroutine = new Task(AlertCoroutine(message));
    }

    private IEnumerator AlertCoroutine(string message) {
        alertTextView.text = message;
        yield return new WaitForSeconds(4);
        alertTextView.text = "";
    }
    
    [Client]
    private void OnMeleeUpgradeDamageClick() {
        // dedicated if (!isClient) return;
        UpgradeManager.Instance.CmdUpgrade(PlayerController.localPlayer.playerId, CommonUpgradeType.Melee);
        
        Debug.Log("Upgrade melee clicked");
    }
    
    [Client]
    private void OnRangeUpgradeDamageClick() {
        // dedicated if (!isClient) return;
        UpgradeManager.Instance.CmdUpgrade(PlayerController.localPlayer.playerId, CommonUpgradeType.Range);
        
        Debug.Log("Upgrade range clicked");
    }
    
    [Client]
    private void OnUpgradeHealthClick() {
        // dedicated if (!isClient) return;
        UpgradeManager.Instance.CmdUpgrade(PlayerController.localPlayer.playerId, CommonUpgradeType.Survivability);
        
        Debug.Log("Upgrade health clicked");
    }
    
    [Client]
    private void OnHeroClick() {
        // dedicated if (!isClient) return;
        SpawnManager.Instance.SpawnHero(PlayerController.localPlayer.playerId);
        
        Debug.Log("Spawn hero clicked");
    }
}
}
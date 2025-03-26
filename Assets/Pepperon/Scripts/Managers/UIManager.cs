using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Mirror;
using Pepperon.Scripts.Controllers;
using Pepperon.Scripts.Entities.Components;
using Pepperon.Scripts.Entities.Systems.LoreSystem.Base.Upgrades;
using Pepperon.Scripts.Systems.LoreSystem.Base.Entities;
using Pepperon.Scripts.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
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

    public TMP_Text gameTimerTextView;

    private void OnGameTimerChanged(string oldGameTimer, string newGameTimer) { }

    public Transform heroItemsParent;
    public GameObject heroItemPrefab;
    private readonly Dictionary<Hero, GameObject> heroItems = new();

    private void Awake() {
        PlayerController.OnLocalPlayerReady += () => {
            PlayerController.localPlayer.OnNewGold += gold => goldTextView.text = gold.ToString();
            PlayerController.localPlayer.OnNewMessage += OnMessageReceived;

            PlayerController.localPlayer.OnRaceChanged += () => {
                InitUpgradesTab();
                InitHeroesTab();
            };
        };

        SessionManager.OnTimeTick += (time) => { gameTimerTextView.text = time; };
        ExperienceComponent.OnNewLevel += (playerId, hero, level) => {
            if (PlayerController.localPlayer.playerId != playerId) return;
            heroItems[hero].GetComponentsInChildren<TMP_Text>().First().text = level.ToString();
        };
    }

    private void InitUpgradesTab() {
        
    }

    private void InitHeroesTab() {
        for (var i = 0; i < PlayerController.localPlayer.race.entities[CommonEntityType.Heroes].Count; i++) {
            var hero = (PlayerController.localPlayer.race.entities[CommonEntityType.Heroes][i] as Hero)!;
            var heroIndex = i;
            var heroItem = Instantiate(heroItemPrefab, heroItemsParent);
            heroItem.GetComponentsInChildren<TMP_Text>().First().text = "1";
            heroItem.GetComponentsInChildren<Image>().First(component => component.name == "HeroIcon")
                .sprite = hero.icon;
            heroItem.GetComponentsInChildren<Button>().First().onClick.AddListener(() => {
                InteractionManager.Instance.EnterChooseBarrackMode(heroIndex);
            });

            heroItems[hero] = heroItem;
        }
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
        // SpawnManager.Instance.SpawnHero(PlayerController.localPlayer.playerId);

        Debug.Log("Spawn hero clicked");
    }
}
}
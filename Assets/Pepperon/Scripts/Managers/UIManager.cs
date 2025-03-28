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

    public Transform primaryUpgradesParent;
    public Transform secondaryUpgradesParent;
    public GameObject upgradeItemPrefab;
    private readonly Dictionary<CommonUpgradeType, GameObject> upgradeItems = new();

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
        UpgradeManager.OnUpgradeEnd += (playerId, upgradeType) => {
            if (PlayerController.localPlayer.playerId != playerId) return;
            upgradeItems[upgradeType].GetComponentsInChildren<Slider>().First().value = 0f;
            upgradeItems[upgradeType].GetComponentsInChildren<Button>().First().interactable = true;
        };
    }

    private void InitUpgradesTab() {
        foreach (var (upgradeType, upgrade) in PlayerController.localPlayer.race.upgrades) {
            var upgradeItem = Instantiate(upgradeItemPrefab, primaryUpgradesParent);
            upgradeItem.GetComponentsInChildren<Image>().First(component => component.name == "UpgradeIcon").sprite =
                upgrade.icon;
            upgradeItem.GetComponentsInChildren<Button>().First().onClick.AddListener(() => { 
                // Local check (todo disable button if not allowed)
                var upgradeProgress = PlayerController.localPlayer.progress.upgrades[upgradeType];
                if (upgradeProgress.progress + 1 > upgrade.progressCosts.Length - 1) return;
                if (PlayerController.localPlayer.gold < upgrade.progressCosts[upgradeProgress.progress + 1]) return;
                
                // Request server to upgrade
                UpgradeManager.Instance.CmdUpgrade(PlayerController.localPlayer.playerId, upgradeType);
                // Local start slider progress
                var progressTime = upgrade.progressTimes[upgradeProgress.progress + 1];
                new Task(PeriodicFunction(progressTime, 1f, progress => {
                    upgradeItem.GetComponentsInChildren<Slider>().First().value = progress / progressTime;
                }));
                // Disable button
                upgradeItem.GetComponentsInChildren<Button>().First().interactable = false;
            });
            
            upgradeItems[upgradeType] = upgradeItem;
        }
    }

    private IEnumerator PeriodicFunction(float totalDuration, float interval, System.Action<float> action) {
        float elapsedTime = 0f;

        while (elapsedTime < totalDuration) {
            action.Invoke(elapsedTime);
            yield return new WaitForSeconds(interval);
            elapsedTime += interval;
        }
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
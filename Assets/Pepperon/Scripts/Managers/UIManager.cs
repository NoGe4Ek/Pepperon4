using System.Collections;
using System.Linq;
using Mirror;
using Pepperon.Scripts.Controllers;
using Pepperon.Scripts.Entities.Systems.LoreSystem.Base.Entities;
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

        private Task gameTimerCoroutine;
        public TMP_Text gameTimerTextView;

        [SyncVar(hook = nameof(OnGameTimerChanged))]
        public string gameTimer;

        private void OnGameTimerChanged(string oldGameTimer, string newGameTimer) {
            gameTimerTextView.text = newGameTimer;
        }

        public Transform heroItems;
        public GameObject heroItemPrefab;

        private void Awake() {
            PlayerController.OnLocalPlayerReady += () => {
                PlayerController.localPlayer.OnNewGold += gold => goldTextView.text = gold.ToString();
                PlayerController.localPlayer.OnNewMessage += OnMessageReceived;

                PlayerController.localPlayer.OnRaceChanged += () => {
                    foreach (var entity in PlayerController.localPlayer.race.entities[CommonEntityType.Heroes]) {
                        var hero = (entity as Hero)!;
                        var heroItem = Instantiate(heroItemPrefab, heroItems);
                        heroItem.GetComponentsInChildren<Image>().First(component => component.name == "HeroIcon")
                                .sprite = hero.icon;
                        heroItem.GetComponentsInChildren<Button>().First().onClick.AddListener(() => {
                            Debug.Log("Click on hero: " + hero.heroName);
                        });
                    }
                };
            };
        }

        private void Start() {
            meleeUpgradeDamageButton.onClick.AddListener(OnMeleeUpgradeDamageClick);
            rangeUpgradeDamageButton.onClick.AddListener(OnRangeUpgradeDamageClick);
            survivabilityUpgradeDamageButton.onClick.AddListener(OnUpgradeHealthClick);
            heroButton.onClick.AddListener(OnHeroClick);
        }

        private void Update() {
            if (!isServer) return;

            if (SessionManager.Instance.isGameNow && (gameTimerCoroutine == null || !gameTimerCoroutine.Running)) {
                gameTimerCoroutine = new Task(UpdateGameTimer());
            }
        }

        private IEnumerator UpdateGameTimer() {
            float gameTime = 0f;
            float lastUpdateTime = Time.time;

            while (SessionManager.Instance.isGameNow) {
                float currentTime = Time.time;
                gameTime += currentTime - lastUpdateTime;
                lastUpdateTime = currentTime;

                int minutes = Mathf.FloorToInt(gameTime / 60);
                int seconds = Mathf.FloorToInt(gameTime % 60);

                gameTimer = $"{minutes:00}:{seconds:00}";

                yield return new WaitForSeconds(0.5f);
            }
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
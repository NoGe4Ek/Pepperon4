using System;
using System.Collections.Generic;
using System.Linq;
using Pepperon.Scripts.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using Button = UnityEngine.UI.Button;

namespace Pepperon.Scripts.UI {
    public class LobbyScreenArgs {
        public string LobbyId { get; set; }
    }

    public class LobbyScreen : BaseScreen {
        public TMP_Text lobbyText;
        public Button readyButton, startButton, disconnectButton;
        public Transform playersContainer;
        public GameObject playerLobbyItemPrefab;
        public Button humanButton, orcButton;
        public string lobbyId;

        private readonly Dictionary<string, GameObject> players = new();

        public override void Initialize(object args) {
            if (args != null && args is LobbyScreenArgs lobbyArgs) {
                lobbyId = lobbyArgs.LobbyId;
                Debug.Log("Lobby ID: " + lobbyId);
            }

            if (string.IsNullOrEmpty(lobbyId)) {
                var createLobbyRequest = HttpClient.Instance.Wss("wss://www.aphirri.ru/ws/lobbies/create",
                    new CreateLobbyRequest("Lobby name"),
                    message => {
                        Debug.Log("Event: " + message);

                        switch (message) {
                            case PlayerConnected playerConnected:
                                UpdateAll(playerConnected);
                                break;
                            case PlayerDisconnected playerDisconnected:
                                LeaveLobby();
                                ScreenManager.Instance.ShowScreen<LobbiesScreen>();
                                break;
                            case RaceChanged raceChanged:
                                UpdateRace(players[raceChanged.UserId], raceChanged.Race);
                                break;
                            case GameStarted gameStarted:
                                PlayerPrefs.SetString("MatchAddress", "213.21.27.138");
                                PlayerPrefs.SetString("PlayerId", gameStarted.UserId);
                                PlayerPrefs.Save();

                                SceneManager.LoadScene("NetworkTest");
                                break;
                            default:
                                throw new ArgumentOutOfRangeException();
                        }
                    },
                    error => { Debug.Log("Error: " + error); });
                new Task(createLobbyRequest);
            }
            else {
                var joinLobbyRequest = HttpClient.Instance.Wss("wss://www.aphirri.ru/ws/lobbies/join",
                    new JoinLobbyRequest(lobbyId),
                    message => {
                        Debug.Log("Event: " + message);

                        switch (message) {
                            case PlayerConnected playerConnected:
                                UpdateAll(playerConnected);
                                break;
                            case PlayerDisconnected playerDisconnected:
                                LeaveLobby();
                                ScreenManager.Instance.ShowScreen<LobbiesScreen>();
                                break;
                            case RaceChanged raceChanged:
                                UpdateRace(players[raceChanged.UserId], raceChanged.Race);
                                break;
                            case GameStarted gameStarted:
                                PlayerPrefs.SetString("MatchAddress", "213.21.27.138");
                                PlayerPrefs.SetString("PlayerId", gameStarted.UserId);
                                PlayerPrefs.Save();

                                SceneManager.LoadScene("NetworkTest");
                                break;
                            default:
                                throw new ArgumentOutOfRangeException();
                        }
                    },
                    error => { Debug.Log("Error: " + error); });
                new Task(joinLobbyRequest);
            }
        }

        private void Start() {
            startButton.onClick.AddListener(() => {
                var startMatchRequest = HttpClient.Instance.Post<MatchResponse>(
                    "https://www.aphirri.ru/matches/" + lobbyId,
                    null,
                    response => { Debug.Log("Response: " + response); },
                    error => { Debug.Log("Error: " + error); });
                new Task(startMatchRequest);
            });
            disconnectButton.onClick.AddListener(() => {
                LeaveLobby();
                ScreenManager.Instance.ShowScreen<LobbiesScreen>();
            });
            humanButton.onClick.AddListener(() => { UpdateRace(Race.HUMANITY); });
            orcButton.onClick.AddListener(() => { UpdateRace(Race.ORC); });
        }

        private void UpdateRace(Race race) {
            var updateRaceRequest = HttpClient.Instance.WssSend(new ChangeRace(race));
            new Task(updateRaceRequest);
        }

        private void UpdateAll(PlayerConnected playerConnected) {
            lobbyId = playerConnected.Lobby.id;
            lobbyText.text = playerConnected.Lobby.name;

            foreach (var idToInstance in players) {
                Destroy(idToInstance.Value);
            }

            players.Clear();

            foreach (var player in playerConnected.Lobby.players) {
                AddNewPlayer(player);
            }
        }

        private void AddNewPlayer(PlayerResponse player) {
            var playerLobbyItem = Instantiate(playerLobbyItemPrefab, playersContainer);
            players.Add(player.user.id, playerLobbyItem);
            if (player.role == LobbyRole.HOST) {
                playerLobbyItem.GetComponentsInChildren<TMP_Text>().First(component => component.name == "UsernameText")
                    .color = Color.red;
                playerLobbyItem.GetComponentsInChildren<TMP_Text>().First(component => component.name == "RaceText")
                        .color =
                    Color.red;
            }

            playerLobbyItem.GetComponentsInChildren<TMP_Text>().First(component => component.name == "UsernameText")
                    .text =
                player.user.username;
            UpdateRace(playerLobbyItem, player.race);
        }

        private void UpdateRace(GameObject playerLobbyItem, Race race) {
            playerLobbyItem.GetComponentsInChildren<TMP_Text>().First(component => component.name == "RaceText").text =
                race.ToString();
        }

        private void OnDestroy() {
            // LeaveLobby();
        }

        private void LeaveLobby() {
            if (string.IsNullOrEmpty(lobbyId)) return;
            
            var leaveLobbyRequest = HttpClient.Instance.WssSend(new LeaveLobby());
            new Task(leaveLobbyRequest);

            lobbyId = "";
            foreach (var idToInstance in players) {
                Destroy(idToInstance.Value);
            }

            players.Clear();
        }
    }

    public struct EmptyResponse { }

    [Serializable]
    public class MatchResponse {
        public string port;

        public MatchResponse(string port) {
            this.port = port;
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using Pepperon.Scripts.Managers;
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
            var createLobbyRequest = HttpClient.Instance.Sse("https://www.aphirri.ru/lobbies/sse",
                new CreateLobbyRequest("Lobby name"),
                message => {
                    Debug.Log("Message type: " + message.type + "; Message extra: " + message.extra);

                    switch (message.type) {
                        case SseEventType.Connected:
                            lobbyId = message.extra;
                            UpdateLobby();
                            break;
                        case SseEventType.Update:
                            UpdateLobby();
                            break;
                        case SseEventType.Disconnected:
                            LeaveLobby();
                            ScreenManager.Instance.ShowScreen<LobbiesScreen>();
                            break;
                        case SseEventType.Start:
                            PlayerPrefs.SetString("MatchAddress", "213.21.27.138");
                            PlayerPrefs.SetString("PlayerId", message.extra);
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
            var joinLobbyRequest = HttpClient.Instance.Sse("https://www.aphirri.ru/lobbies/sse/join/" + lobbyId,
                null,
                message => {
                    Debug.Log("Message: " + message);

                    switch (message.type) {
                        case SseEventType.Connected:
                            UpdateLobby();
                            break;
                        case SseEventType.Update:
                            UpdateLobby();
                            break;
                        case SseEventType.Disconnected:
                            LeaveLobby();
                            ScreenManager.Instance.ShowScreen<LobbiesScreen>();
                            break;
                        case SseEventType.Start:
                            PlayerPrefs.SetString("MatchAddress", "213.21.27.138");
                            PlayerPrefs.SetString("PlayerId", message.extra);
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
            var startMatchRequest = HttpClient.Instance.Post<MatchResponse>("https://www.aphirri.ru/matches/" + lobbyId,
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
        var updateRaceRequest = HttpClient.Instance.Post<EmptyResponse>(
            "https://www.aphirri.ru/lobbies/" + lobbyId + "/race",
            new ChangeRaceRequest(race),
            response => { Debug.Log("Get race response: " + response); },
            error => { Debug.Log("Error: " + error); });
        new Task(updateRaceRequest);
    }

    private void UpdateLobby() {
        var getLobbyRequest = HttpClient.Instance.Get<LobbyResponse>("https://www.aphirri.ru/lobbies/" + lobbyId,
            null,
            response => {
                Debug.Log("Get lobby response: " + response);
                lobbyText.text = response.name + ": " + response.id;

                foreach (var idToInstance in players) {
                    Destroy(idToInstance.Value);
                }

                players.Clear();

                foreach (var player in response.players) {
                    AddNewPlayer(player);
                }
            },
            error => {
                Debug.Log("Error: " + error);
                if (!error.Contains("404")) return;

                LeaveLobby();
                ScreenManager.Instance.ShowScreen<LobbiesScreen>();
            });
        new Task(getLobbyRequest);
    }

    private void AddNewPlayer(PlayerResponse player) {
        var playerLobbyItem = Instantiate(playerLobbyItemPrefab, playersContainer);
        players.Add(player.user.id, playerLobbyItem);
        if (player.role == LobbyRole.HOST) {
            playerLobbyItem.GetComponentsInChildren<TMP_Text>().First(component => component.name == "UsernameText")
                .color = Color.red;
            playerLobbyItem.GetComponentsInChildren<TMP_Text>().First(component => component.name == "RaceText").color =
                Color.red;
        }

        playerLobbyItem.GetComponentsInChildren<TMP_Text>().First(component => component.name == "UsernameText").text =
            player.user.username;
        playerLobbyItem.GetComponentsInChildren<TMP_Text>().First(component => component.name == "RaceText").text =
            player.race.ToString();
    }

    private void OnDestroy() {
        LeaveLobby();
    }

    private void LeaveLobby() {
        if (string.IsNullOrEmpty(lobbyId)) return;

        var leaveLobbyRequest = HttpClient.Instance.Post<EmptyResponse>(
            "https://www.aphirri.ru/lobbies/leave/" + lobbyId,
            null,
            response => { Debug.Log("Leave lobby response: " + response); },
            error => { Debug.Log("Error: " + error); });
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

[Serializable]
public class ChangeRaceRequest {
    public Race race;

    public ChangeRaceRequest(Race race) {
        this.race = race;
    }
}

[Serializable]
public class SseEvent {
    public SseEventType type;
    public string extra;

    public SseEvent(SseEventType type, string extra) {
        this.type = type;
        this.extra = extra;
    }
}

[Serializable]
public enum SseEventType {
    Connected,
    Disconnected,
    Update,
    Start
}
}
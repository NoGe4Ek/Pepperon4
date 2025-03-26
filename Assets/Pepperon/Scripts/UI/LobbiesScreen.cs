using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Pepperon.Scripts.Utils;
using TMPro;
using UnityEngine;
using Button = UnityEngine.UI.Button;

namespace Pepperon.Scripts.UI {
    public class LobbiesScreen : BaseScreen {
        public TMP_Text usernameText;
        public Button joinButton, createButton, updateButton;
        public Transform lobbiesContainer;
        public GameObject lobbyItemPrefab;

        private readonly Dictionary<string, GameObject> lobbies = new();

        public override void Initialize(object args) {
            UpdateLobbies();
        }

        private void Start() {
            var profileRequest = HttpClient.Instance.Get<UserResponse>("https://www.aphirri.ru/users/profile",
                null,
                response => {
                    Debug.Log("Response: " + response);
                    usernameText.text = response.username;
                },
                error => { Debug.Log("Error: " + error); });
            new Task(profileRequest);

            createButton.onClick.AddListener(() => { ScreenManager.Instance.ShowScreen<LobbyScreen>(); });

            updateButton.onClick.AddListener(UpdateLobbies);
        }

        private void UpdateLobbies() {
            var lobbiesRequest = HttpClient.Instance.Get<LobbiesResponse>("https://www.aphirri.ru/lobbies",
                null,
                response => {
                    Debug.Log("Response: " + response);
                    foreach (var lobby in lobbies) {
                        Destroy(lobby.Value);
                    }

                    lobbies.Clear();
                    if (response.total > 0)
                        foreach (var lobby in response.items) {
                            AddLobby(lobby.id, lobby.name, lobby.players.Count, lobby.players.Count);
                        }
                },
                error => { Debug.Log("Error: " + error); });
            new Task(lobbiesRequest);
        }

        public void AddLobby(string lobbyId, string lobbyName, int playersCount, int maxPlayers) {
            var lobbyItem = Instantiate(lobbyItemPrefab, lobbiesContainer);
            lobbies.Add(lobbyId, lobbyItem);
            lobbyItem.GetComponentsInChildren<TMP_Text>().First(component => component.name == "NameText").text =
                lobbyName;
            lobbyItem.GetComponentsInChildren<TMP_Text>().First(component => component.name == "PlayersCountText")
                .text = playersCount + "/" + maxPlayers;

            lobbyItem.GetComponent<Button>().onClick.AddListener(() => {
                Debug.Log("Click on lobby: " + lobbyId);
                ScreenManager.Instance.ShowScreen<LobbyScreen>(new LobbyScreenArgs { LobbyId = lobbyId });
            });
        }
    }

    [Serializable]
    public class CreateLobbyRequest {
        public string Name { get; set; }

        public CreateLobbyRequest(string name) {
            Name = name;
        }
    }

    [Serializable]
    public class JoinLobbyRequest {
        public string Id { get; set; }

        public JoinLobbyRequest(string id) {
            Id = id;
        }
    }

    [Serializable]
    public class LobbiesResponse {
        public List<LobbyResponse> items { get; set; }
        public int total { get; set; }
    }

    [Serializable]
    public class LobbyResponse {
        public string id { get; set; }
        public string name { get; set; }
        public string hostUserId { get; set; }
        public List<PlayerResponse> players { get; set; }
    }
    
    [Serializable]
    public class BotResponse {
        public string name { get; set; }
        public Race race { get; set; }
        
        public BotResponse(string name, Race race) {
            this.name = name;
            this.race = race;
        }
    }

    [Serializable]
    public class PlayerResponse {
        public UserResponse user { get; set; }
        public LobbyRole role { get; set; }
        public Race race { get; set; }
    }

    [Serializable]
    public class UserResponse {
        public string id { get; set; }
        public string username { get; set; }
        public int rating { get; set; }
    }

    [Serializable]
    [JsonConverter(typeof(StringEnumConverter))]
    public enum LobbyRole {
        [JsonProperty("HOST")] HOST,
        [JsonProperty("MEMBER")] MEMBER
    }

    [Serializable]
    [JsonConverter(typeof(StringEnumConverter))]
    public enum Race {
        [JsonProperty("NONE")] NONE,
        [JsonProperty("HUMANITY")] HUMANITY,
        [JsonProperty("ORC")] ORC
    }
}
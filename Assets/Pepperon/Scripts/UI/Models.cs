using System;
using System.Linq;
using System.Reflection;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Pepperon.Scripts.UI {
    public interface IServerSessionEvent { }

    public interface ILobbyServerSessionEvent : IServerSessionEvent { }

    public interface ILobbyClientSessionEvent : IServerSessionEvent { }

    [Serializable]
    public class ChangeRace : ILobbyClientSessionEvent {
        public Race race;

        public ChangeRace(Race race) {
            this.race = race;
        }
    }

    public class LeaveLobby : ILobbyClientSessionEvent { }

    [Serializable]
    public class PlayerConnected : ILobbyServerSessionEvent {
        public string UserId { get; set; }
        public LobbyResponse Lobby { get; set; }

        public PlayerConnected(string userId, LobbyResponse lobby) {
            Lobby = lobby;
            UserId = userId;
        }
    }

    [Serializable]
    public class PlayerDisconnected : ILobbyServerSessionEvent {
        public string UserId { get; set; }

        public PlayerDisconnected(string userId) {
            UserId = userId;
        }
    }

    [Serializable]
    public class RaceChanged : ILobbyServerSessionEvent {
        public string UserId { get; set; }
        public Race Race { get; set; }

        public RaceChanged(string userId, Race race) {
            UserId = userId;
            Race = race;
        }
    }

    [Serializable]
    public class GameStarted : ILobbyServerSessionEvent {
        public string UserId { get; set; }

        public GameStarted(string userId) {
            UserId = userId;
        }
    }

    class Models {
        public static IServerSessionEvent ToEvent(string json) {
            try {
                var jsonObject = JsonConvert.DeserializeObject<JObject>(json);
                string type = jsonObject["type"]?.ToString();

                var typeDictionary = Assembly.GetExecutingAssembly().GetTypes()
                    .Where(t => typeof(ILobbyServerSessionEvent).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract)
                    .ToDictionary(t => t.Name, t => t);

                return (IServerSessionEvent)JsonConvert.DeserializeObject(json, typeDictionary[type]);
            }
            catch (Exception ex) {
                Console.WriteLine($"Error deserializing event: {ex.Message}");
                return null;
            }
        }
        
        public static string ToMessage(ILobbyClientSessionEvent eventObject)
        {
            string json = JsonConvert.SerializeObject(eventObject);
            int jsonStartIndex = json.IndexOf('{');
            string typeName = eventObject.GetType().Name;
            
            var newJson = new StringBuilder();
            newJson.Append(json.Substring(0, jsonStartIndex + 1));
            newJson.Append($"\"type\":\"{typeName}\"");
            if (json[jsonStartIndex + 1] != '}')
                newJson.Append(",");
            newJson.Append(json.Substring(jsonStartIndex + 1));

            return newJson.ToString();
        }
    }
}
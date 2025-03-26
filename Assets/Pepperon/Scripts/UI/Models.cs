using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
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

    [Serializable]
    public class MatchResult {
        public MatchResultType type;
        public String startTime;
        public String endTime;
        public int duration;
        public string winnerId;
        public Dictionary<string, PlayerStatistic> playerStatistics;

        public MatchResult(MatchResultType type, DateTimeOffset startTime, DateTimeOffset endTime, TimeSpan duration,
            string winnerId, Dictionary<string, PlayerStatistic> playerStatistics) {
            this.type = type;
            this.startTime = startTime.ToString("yyyy-MM-ddTHH:mm:ss.ffffffZ");
            this.endTime = endTime.ToString("yyyy-MM-ddTHH:mm:ss.ffffffZ");
            this.duration = Convert.ToInt32(Math.Floor(duration.TotalSeconds));
            this.winnerId = winnerId;
            this.playerStatistics = playerStatistics;
        }
    }

    [Serializable]
    public class PlayerStatistic {
        public int TotalGold { get; set; }

        public PlayerStatistic(int totalGold) {
            TotalGold = totalGold;
        }
    }

    [Serializable]
    [JsonConverter(typeof(StringEnumConverter))]
    public enum MatchResultType {
        [JsonProperty("NORMAL")] NORMAL,

        [JsonProperty("ABNORMAL")] ABNORMAL
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
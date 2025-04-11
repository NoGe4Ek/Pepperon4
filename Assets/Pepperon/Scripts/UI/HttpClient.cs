using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NativeWebSocket;
using Newtonsoft.Json;
using Pepperon.Scripts.Utils;
using UnityEngine;
using UnityEngine.Networking;

namespace Pepperon.Scripts.UI {
    public class HttpClient : MonoBehaviour {
        public static HttpClient Instance { get; private set; }

        private void Awake() {
            Instance = this;
        }

        public string bearerToken;
        public string userId;
        private readonly List<WebSocket> websockets = new();

        private async void OnApplicationQuit() {
            foreach (var websocket in websockets) {
                await websocket.Close();
            }
        }

        private static string BodyToParams(object body) {
            var parameters = new Dictionary<string, string>();
            foreach (var property in body.GetType().GetProperties()) {
                var key = Uri.EscapeDataString(Char.ToLower(property.Name[0]) + property.Name.Substring(1));
                var value = Uri.EscapeDataString(property.GetValue(body)?.ToString() ?? string.Empty);
                parameters.Add(key, value);
            }

            return string.Join("&", parameters.Select(kv => $"{kv.Key}={kv.Value}"));
        }
        
        public IEnumerator WssClose() {
            websockets.RemoveAll(it => it.State == WebSocketState.Closed);
            foreach (var websocket in websockets) {
                yield return new Task(CloseWebSocket(websocket));
            }
            websockets.Clear();
        }

        public IEnumerator WssSend(ILobbyClientSessionEvent message) {
            foreach (var websocket in websockets) {
                yield return new Task(SendWebSocket(websocket, message));
            }
        }

        public IEnumerator Wss(string url, object body, Action<IServerSessionEvent> onMessage, Action<string> onError,
            Action<WebSocketCloseCode> onClose) {
            // Auth header not work for WebGL Build!
            // new Dictionary<string, string> {
            //     { "Authorization", $"Bearer {bearerToken}" }
            // }
            // https://github.com/endel/NativeWebSocket/issues/14
            WebSocket websocket = new WebSocket(
                url + "?" + BodyToParams(body)
            );
            websockets.Add(websocket);
            websocket.OnOpen += () => {
                new Task(WssSend(new Auth(bearerToken)));
                Debug.Log("Connection open!");
            };

            websocket.OnError += (e) => {
                Debug.Log("Error! " + e);
                onError.Invoke(e);
            };

            websocket.OnClose += (e) => {
                Debug.Log("Connection closed! Reason: " + e);
                websockets.Remove(websocket);
                onClose.Invoke(e);
            };

            websocket.OnMessage += (bytes) => {
                Debug.Log("OnMessage!");
                Debug.Log(bytes);
                var message = Encoding.UTF8.GetString(bytes);
                var json = message.Replace("data: ", "");
                IServerSessionEvent e = Models.ToEvent(json);
                onMessage?.Invoke(e);
            };

            yield return new Task(ConnectWebSocket(websocket));
        }

        private IEnumerator SendWebSocket(WebSocket websocket, ILobbyClientSessionEvent message) {
            if (websocket.State != WebSocketState.Open) yield break;

            var sendTask = websocket.SendText(Models.ToMessage(message));
            while (!sendTask.IsCompleted)
                yield return null;

            if (sendTask.Exception != null)
                Debug.LogError("WebSocket Send Failed: " + sendTask.Exception.Message);
        }

        private IEnumerator ConnectWebSocket(WebSocket websocket) {
            var connectTask = websocket.Connect();
            while (!connectTask.IsCompleted)
                yield return null;

            if (connectTask.Exception != null)
                Debug.LogError("WebSocket Connection Failed: " + connectTask.Exception.Message);
        }

        private IEnumerator CloseWebSocket(WebSocket websocket) {
            if (websocket != null) {
                var closeTask = websocket.Close();
                websockets.Remove(websocket);
                while (!closeTask.IsCompleted)
                    yield return null;
            }
        }

        private void Update() {
#if !UNITY_WEBGL || UNITY_EDITOR
            foreach (var websocket in websockets) {
                websocket?.DispatchMessageQueue();
            }
#endif
        }

        public IEnumerator Sse(string url, object body, Action<IServerSessionEvent> onSuccess, Action<string> onError) {
            using (UnityWebRequest request = UnityWebRequest.Get(url)) {
                AddAuthorizationHeader(request);

                if (body != null) {
                    string json = JsonConvert.SerializeObject(body);
                    byte[] bodyRaw = Encoding.UTF8.GetBytes(json);
                    request.uploadHandler = new UploadHandlerRaw(bodyRaw);
                    request.SetRequestHeader("Content-Type", "application/json");
                }

                request.downloadHandler = new LobbySseDownloadHandler(message => {
                    if (string.IsNullOrWhiteSpace(message)) return;

                    var json = message.Replace("data: ", "");
                    IServerSessionEvent e = Models.ToEvent(json);
                    onSuccess?.Invoke(e);
                    if (e is PlayerDisconnected)
                        request.downloadHandler.Dispose();
                });

                yield return request.SendWebRequest();

                if (request.result is UnityWebRequest.Result.ConnectionError or UnityWebRequest.Result.ProtocolError) {
                    onError?.Invoke(request.error);
                }
            }
        }

        public IEnumerator Get<T>(string url, object body, Action<T> onSuccess, Action<string> onError) {
            using (UnityWebRequest request = UnityWebRequest.Get(url)) {
                AddAuthorizationHeader(request);

                if (body != null) {
                    string json = JsonConvert.SerializeObject(body);
                    byte[] bodyRaw = Encoding.UTF8.GetBytes(json);
                    request.uploadHandler = new UploadHandlerRaw(bodyRaw);
                    request.SetRequestHeader("Content-Type", "application/json");
                }

                request.downloadHandler = new DownloadHandlerBuffer();

                yield return request.SendWebRequest();

                if (request.result is UnityWebRequest.Result.ConnectionError or UnityWebRequest.Result.ProtocolError) {
                    onError?.Invoke(request.error);
                }
                else {
                    string responseText = request.downloadHandler.text;
                    T response = JsonConvert.DeserializeObject<T>(responseText);

                    onSuccess?.Invoke(response);
                }
            }
        }

        public IEnumerator Post<T>(string url, object body, Action<T> onSuccess, Action<string> onError) {
            using (UnityWebRequest request = new UnityWebRequest(url, "POST")) {
                AddAuthorizationHeader(request);

                if (body != null) {
                    string json = JsonConvert.SerializeObject(body);
                    Debug.Log("Request to: " + url + "; JSON: " + json);
                    byte[] bodyRaw = Encoding.UTF8.GetBytes(json);
                    request.uploadHandler = new UploadHandlerRaw(bodyRaw);
                    request.SetRequestHeader("Content-Type", "application/json");
                }

                request.downloadHandler = new DownloadHandlerBuffer();

                yield return request.SendWebRequest();

                if (request.result is UnityWebRequest.Result.ConnectionError or UnityWebRequest.Result.ProtocolError) {
                    onError?.Invoke(request.error);
                }
                else {
                    if (typeof(T) == typeof(EmptyResponse)) {
                        onSuccess?.Invoke(default);
                    }
                    else {
                        string responseText = request.downloadHandler.text;
                        T response = JsonConvert.DeserializeObject<T>(responseText);
                        onSuccess?.Invoke(response);
                    }
                }
            }
        }

        private void AddAuthorizationHeader(UnityWebRequest request) {
            if (!string.IsNullOrEmpty(bearerToken)) {
                request.SetRequestHeader("Authorization", "Bearer " + bearerToken);
            }
        }
    }

    public class LobbySseDownloadHandler : SseDownloadHandlerBase {
        private readonly Action<string> onMessageReceived;

        public LobbySseDownloadHandler(Action<string> onMessageReceived) : base(new byte[1024]) {
            this.onMessageReceived = onMessageReceived;
        }

        protected override void OnNewLineReceived(string line) {
            Debug.Log(line);
            onMessageReceived?.Invoke(line);
        }
    }

    public abstract class SseDownloadHandlerBase : DownloadHandlerScript {
        private readonly StringBuilder _currentLine = new();

        protected SseDownloadHandlerBase(byte[] buffer) : base(buffer) { }

        protected abstract void OnNewLineReceived(string line);

        protected override bool ReceiveData(byte[] data, int dataLength) {
            for (var i = 0; i < dataLength; i++) {
                var b = data[i];
                if (b == '\n') {
                    OnNewLineReceived(_currentLine.ToString());
                    _currentLine.Clear();
                }
                else {
                    _currentLine.Append((char)b);
                }
            }

            return true;
        }

        protected override void CompleteContent() {
            if (_currentLine.Length > 0)
                OnNewLineReceived(_currentLine.ToString());
        }
    }
}
using System;
using System.Collections;
using System.Text;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;

namespace Pepperon.Scripts.UI {
public class HttpClient : MonoBehaviour {
    public static HttpClient Instance { get; private set; }

    private void Awake() {
        Instance = this;
    }

    private string bearerToken;

    public void SetBearerToken(string token) {
        bearerToken = token;
    }

    public IEnumerator Sse(string url, object body, Action<SseEvent> onSuccess, Action<string> onError) {
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

                var sseEventJson = message.Replace("data: ", "");
                SseEvent response = JsonConvert.DeserializeObject<SseEvent>(sseEventJson);
                onSuccess?.Invoke(response);
                if (response.type == SseEventType.Disconnected)
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
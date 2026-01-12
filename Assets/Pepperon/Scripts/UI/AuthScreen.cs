using System;
using Pepperon.Scripts.Utils;
using TMPro;
using UnityEngine;
using Button = UnityEngine.UI.Button;

namespace Pepperon.Scripts.UI {
public class AuthScreen : BaseScreen {
    public Button loginButton, signUpButton;
    public TMP_InputField usernameTextField, passwordTextField;

    public override void Awake() {
        base.Awake();
        signUpButton.onClick.AddListener(() => {
            var register = HttpClient.Instance.Post<AuthResponse>("https://www.aphirri.ru/users/register",
                new AuthRequest(usernameTextField.text, passwordTextField.text),
                response => {
                    Debug.Log("Response: " + response.token);
                    HttpClient.Instance.bearerToken = response.token;
                    HttpClient.Instance.userId = response.userId;
                    
                    ScreenManager.Instance.ShowScreen<LobbiesScreen>();
                },
                error => { Debug.Log("Error: " + error); });
            new Task(register);
        });
        loginButton.onClick.AddListener(() => {
            var login = HttpClient.Instance.Post<AuthResponse>("https://www.aphirri.ru/users/login",
                new AuthRequest(usernameTextField.text, passwordTextField.text),
                response => {
                    Debug.Log("Response token: " + response.token);
                    Debug.Log("Response ActiveMatchPort: " + response.activeMatchPort);
                    HttpClient.Instance.bearerToken = response.token;
                    HttpClient.Instance.userId = response.userId;
                    PlayerPrefs.SetString("ActiveMatchPort", response.activeMatchPort);
                    
                    ScreenManager.Instance.ShowScreen<LobbiesScreen>();
                },
                error => { Debug.Log("Error: " + error); });
            new Task(login);
        });
    }

    [Serializable]
    public class AuthRequest {
        public string username;
        public string password;

        public AuthRequest(string username, string password) {
            this.username = username;
            this.password = password;
        }
    }

    [Serializable]
    public class AuthResponse {
        public string token;
        public string userId;
        public string activeMatchPort;
    }
}
}
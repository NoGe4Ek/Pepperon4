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
            var register = HttpClient.Instance.Post<TokenResponse>("https://www.aphirri.ru/users/register",
                new AuthRequest(usernameTextField.text, passwordTextField.text),
                response => {
                    Debug.Log("Response: " + response.token);
                    HttpClient.Instance.SetBearerToken(response.token);
                    
                    ScreenManager.Instance.ShowScreen<LobbiesScreen>();
                },
                error => { Debug.Log("Error: " + error); });
            new Task(register);
        });
        loginButton.onClick.AddListener(() => {
            var login = HttpClient.Instance.Post<TokenResponse>("https://www.aphirri.ru/users/login",
                new AuthRequest(usernameTextField.text, passwordTextField.text),
                response => {
                    Debug.Log("Response: " + response.token);
                    HttpClient.Instance.SetBearerToken(response.token);
                    
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
    public class TokenResponse {
        public string token;
    }
}
}
using Mirror;
using Pepperon.Scripts.Networking;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Pepperon.Scripts.UI {
public class ConnectScreen : BaseScreen {
    public Button serverClientButton, serverButton, clientButton;

    public TMP_InputField addressTextField, usernameTextField;

    private void Start() {
        //Update the canvas text if you have manually changed network managers address from the game object before starting the game scene
        if (NetworkManager.singleton.networkAddress != "localhost") {
            addressTextField.text = NetworkManager.singleton.networkAddress;
        }

        //Adds a listener to the main input field and invokes a method when the value changes.
        addressTextField.onValueChanged.AddListener(delegate { ValueChangeCheck(); });
        usernameTextField.onValueChanged.AddListener(delegate { OnUsernameChange(); });

        //Make sure to attach these Buttons in the Inspector
        serverClientButton.onClick.AddListener(ButtonServerClient);
        serverButton.onClick.AddListener(ButtonServer);
        clientButton.onClick.AddListener(ButtonClient);

        //This updates the Unity canvas, we have to manually call it every change, unlike legacy OnGUI.
        SetupCanvas();
    }

    // Invoked when the value of the text field changes.
    public void ValueChangeCheck() {
        NetworkManager.singleton.networkAddress = addressTextField.text;
    }
    
    public void OnUsernameChange() {
        PlayerPrefs.SetString("Username", usernameTextField.text);
    }

    public void ButtonServerClient() {
        NetworkManager.singleton.StartHost();
        SetupCanvas();
    }

    public void ButtonServer() {
        NetworkManager.singleton.StartServer();
        SetupCanvas();
    }

    public void ButtonClient() {
        NetworkManager.singleton.StartClient();
        SetupCanvas();
    }

    public void ButtonStop() {
        // stop host if host mode
        if (NetworkServer.active && NetworkClient.isConnected) {
            NetworkManager.singleton.StopHost();
        }
        // stop client if client-only
        else if (NetworkClient.isConnected) {
            NetworkManager.singleton.StopClient();
        }
        // stop server if server-only
        else if (NetworkServer.active) {
            NetworkManager.singleton.StopServer();
        }

        SetupCanvas();
    }

    public void SetupCanvas() {
        if (!NetworkClient.isConnected && !NetworkServer.active) {
            if (NetworkClient.active) {
                ScreenManager.Instance.ShowScreen<LobbyScreen>();
            }
            else {
                ScreenManager.Instance.ShowScreen<ConnectScreen>();
            }
        }
        else {
            ScreenManager.Instance.ShowScreen<LobbyScreen>();
        
            // server / client status message
            if (NetworkServer.active) {
                // serverText.text = "Server: active. Transport: " + Transport.active;
                // // Note, older mirror versions use: Transport.activeTransport
            }
        
            if (NetworkClient.isConnected) {
                // clientText.text = "Client: address=" + NetworkManager.singleton.networkAddress;
            }
        }
    }
}
}
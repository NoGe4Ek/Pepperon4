using System;
using System.Collections.Generic;
using UnityEngine;

namespace Pepperon.Scripts.UI {
public class ScreenManager : MonoBehaviour {
    private ScreenManager() { }
    public static ScreenManager Instance { get; private set; }

    private BaseScreen currentScreen;
    public List<BaseScreen> screens;

    private void Awake() {
        Instance = this;
    }

    private void Start() {
        ShowScreen<AuthScreen>();
    }

    public void ShowScreen<T>(object args = null) where T : BaseScreen {
        BaseScreen screenToShow = FindScreenOfType<T>();
        if (screenToShow == null) {
            Debug.LogError($"Screen of type {typeof(T)} not found on the scene.");
            return;
        }

        if (currentScreen != null) {
            currentScreen.Hide();
        }
        
        currentScreen = screenToShow;
        
        currentScreen.Initialize(args);
        currentScreen.Show();
    }

    private BaseScreen FindScreenOfType<T>() where T : BaseScreen {
        foreach (var screen in screens) {
            if (screen is T) {
                return screen;
            }
        }

        return null;
    }
}
}
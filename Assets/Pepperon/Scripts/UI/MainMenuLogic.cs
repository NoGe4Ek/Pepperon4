using UnityEngine;
using UnityEngine.SceneManagement;

namespace Pepperon.Scripts.UI {
public class MainMenuLogic: MonoBehaviour {
    public void OnPlayClick() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void OnExitClick() {
        Application.Quit();
    }
}
}
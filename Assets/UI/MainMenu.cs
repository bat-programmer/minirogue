using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void StartGame()
    {
        // Load scene 1 (make sure it's added to Build Settings)
        SceneManager.LoadScene(1);
    }

    public void OpenOptions()
    {
        // For now, just log or do nothing
        Debug.Log("Options menu opened (not implemented).");
    }

    public void QuitGame()
    {
        Debug.Log("Quitting game...");
        Application.Quit();

#if UNITY_EDITOR
        // This is only for testing inside the Unity Editor
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}

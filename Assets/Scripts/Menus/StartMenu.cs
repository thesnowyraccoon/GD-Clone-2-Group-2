using UnityEngine;
using UnityEngine.SceneManagement;

public class StartMenu : MonoBehaviour
{
    [Header("Scene to load on Play")]
    [Tooltip("Must be added to File > Build Settings > Scenes In Build")]
    public string gameSceneName = "MainGame";

    [Header("Optional Panels")]
    public GameObject mainPanel;     // Play / Options / Quit buttons
    public GameObject optionsPanel;  // shown when Options is clicked

    void Start()
    {
        if (mainPanel != null) mainPanel.SetActive(true);
        if (optionsPanel != null) optionsPanel.SetActive(false);

        Time.timeScale = 1f; // in case a paused game returned to this menu
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    // Hook this up to your "Play" button's OnClick
    public void PlayGame()
    {
        SceneManager.LoadScene(gameSceneName);
    }

    // Hook this up to your "Options" button's OnClick
    public void OpenOptions()
    {
        if (mainPanel != null) mainPanel.SetActive(false);
        if (optionsPanel != null) optionsPanel.SetActive(true);
    }

    // Hook this up to the Options panel's "Back" button OnClick
    public void CloseOptions()
    {
        if (optionsPanel != null) optionsPanel.SetActive(false);
        if (mainPanel != null) mainPanel.SetActive(true);
    }

    // Hook this up to your "Quit" button's OnClick
    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
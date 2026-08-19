using UnityEngine;
using UnityEngine.InputSystem;

public class PauseMenu : MonoBehaviour
{
    [Header("References")]
    public GameObject pauseMenuUI; // the panel/canvas group to show/hide

    private bool isPaused = false;

    void Start()
    {
        if (pauseMenuUI != null) pauseMenuUI.SetActive(false);
    }

    // Wire this to a "Pause" action in your Input Actions asset (e.g. Escape / Start button)
    public void OnPause(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            TogglePause();
        }
    }

    public void TogglePause()
    {
        isPaused = !isPaused;

        if (pauseMenuUI != null) pauseMenuUI.SetActive(isPaused);

        Time.timeScale = isPaused ? 0f : 1f;

        Cursor.lockState = isPaused ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = isPaused;
    }

    // Hook this up to a "Resume" button's OnClick
    public void Resume()
    {
        isPaused = false;
        if (pauseMenuUI != null) pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Hook this up to a "Quit" button's OnClick
    public void QuitGame()
    {
        Time.timeScale = 1f; // reset before quitting, avoids issues if scene reloads
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}

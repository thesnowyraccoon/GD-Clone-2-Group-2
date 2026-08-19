using UnityEngine;
using UnityEngine.SceneManagement;

public class StartMenu : MonoBehaviour
{
    public void Play()
    {
        SceneManager.LoadScene("GameScene");
        Time.timeScale = 1f;
    }

    public void Tutorial(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
        Time.timeScale = 1f;

    }


    public void Quit()
    {
        Application.Quit();
    }
}
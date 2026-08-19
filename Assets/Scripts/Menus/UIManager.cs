using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void onPlayButtonClicked()
    {
        PlayerPrefs.SetString("Scene to go to", "Level - Sabrina");
        SceneManager.LoadScene("Level - Sabrina");
    }
}

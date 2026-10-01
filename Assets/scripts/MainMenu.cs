using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    void OnEnable()
    {
        Time.timeScale = 1f;
    }

    public void Jugar()
    {
        AudioManager.Instance.PlayButton();
        SceneManager.LoadScene("Level_01");
    }

    public void Salir()
    {
        AudioManager.Instance.PlayButton();
        Application.Quit();
    }
}

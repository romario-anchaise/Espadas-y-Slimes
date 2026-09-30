using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] GameObject panelPausa;
    [SerializeField] GameObject primerBoton;
    public static bool EnPausa;

    void Awake()
    {
        if (panelPausa == null)
            panelPausa = GameObject.Find("PausaPanel");

        Time.timeScale = 1f;
        EnPausa = false;
        if (panelPausa != null)
            panelPausa.SetActive(false);
    }

    void Update()
    {
        // Detecta si se presiona la tecla Escape
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            if (EnPausa) Reanudar(); else Pausar(); 
        }
    }

    public void Pausar()
    {
        if (panelPausa == null)
            return;

        panelPausa.SetActive(true);
        Time.timeScale = 0f;
        EnPausa = true;

        if (EventSystem.current != null && primerBoton != null)
        {
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(primerBoton);
        }
    }

    public void Reanudar()
    {
        if (panelPausa != null)
            panelPausa.SetActive(false);
        Time.timeScale = 1f;
        EnPausa = false;
    }

    public void IrAlMenu()
    {
        // Siempre restaurar el tiempo antes de cambiar de escena
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }
}

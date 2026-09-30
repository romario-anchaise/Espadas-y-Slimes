using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [SerializeField] Image barraVida;
    [SerializeField] TMP_Text textoPuntos;
    [SerializeField] Image overlayDanio;
    [SerializeField] TMP_Text avisoPuntos;

    int puntos;
    Coroutine rutinaDanio;
    Coroutine rutinaPuntos;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); 
            return; 
        }
        Instance = this;
    }

    void Start()
    {
        ActualizarPuntos(0);
        if (overlayDanio != null)
            overlayDanio.color = new Color(1f, 0f, 0f, 0f);
        if (avisoPuntos != null)
            avisoPuntos.color = new Color(1f, 0.82f, 0.2f, 0f);
    }

    public void ActualizarVida(int actual, int max)
    {
        if (barraVida == null || max <= 0)
            return;

        barraVida.fillAmount = Mathf.Clamp01((float)actual / max);
    }

    public void ActualizarPuntos(int puntos)
    {
        this.puntos = Mathf.Max(0, puntos);
        if (textoPuntos != null)
            textoPuntos.text = this.puntos.ToString("D4");
    }

    public void SumarPuntos(int cantidad)
    {
        if (cantidad <= 0)
            return;

        ActualizarPuntos(puntos + cantidad);

        if (rutinaPuntos != null)
            StopCoroutine(rutinaPuntos);
        rutinaPuntos = StartCoroutine(RutinaAvisoPuntos(cantidad));
    }

    public void FlashDanio()
    {
        if (overlayDanio == null)
            return;

        if (rutinaDanio != null)
            StopCoroutine(rutinaDanio);
        rutinaDanio = StartCoroutine(RutinaFlashDanio());
    }

    IEnumerator RutinaFlashDanio()
    {
        const float duracion = 0.25f;
        float tiempo = 0f;

        while (tiempo < duracion)
        {
            tiempo += Time.unscaledDeltaTime;
            float alpha = Mathf.Lerp(0.5f, 0f, tiempo / duracion);
            overlayDanio.color = new Color(1f, 0f, 0f, alpha);
            yield return null;
        }

        overlayDanio.color = new Color(1f, 0f, 0f, 0f);
        rutinaDanio = null;
    }

    IEnumerator RutinaAvisoPuntos(int cantidad)
    {
        if (avisoPuntos == null)
            yield break;

        const float duracion = 0.55f;
        RectTransform rect = avisoPuntos.rectTransform;
        Vector2 inicio = new Vector2(0f, -40f);
        Vector2 fin = new Vector2(0f, 30f);
        avisoPuntos.text = $"+{cantidad}";
        float tiempo = 0f;

        while (tiempo < duracion)
        {
            tiempo += Time.unscaledDeltaTime;
            float progreso = Mathf.Clamp01(tiempo / duracion);
            rect.anchoredPosition = Vector2.Lerp(inicio, fin, progreso);
            avisoPuntos.color = new Color(1f, 0.82f, 0.2f, 1f - progreso);
            yield return null;
        }

        avisoPuntos.color = new Color(1f, 0.82f, 0.2f, 0f);
        rutinaPuntos = null;
    }
}

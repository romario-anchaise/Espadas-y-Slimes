using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class PointPickup : MonoBehaviour
{
    [SerializeField] int valor = 100;
    [SerializeField] float amplitud = 0.12f;
    [SerializeField] float velocidad = 2.5f;

    Vector3 posicionInicial;

    void Awake()
    {
        posicionInicial = transform.position;
        Collider2D col = GetComponent<Collider2D>();
        col.isTrigger = true;
    }

    void Update()
    {
        float desplazamiento = Mathf.Sin(Time.time * velocidad) * amplitud;
        transform.position = posicionInicial + Vector3.up * desplazamiento;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        if (UIManager.Instance != null)
            UIManager.Instance.SumarPuntos(valor);

        AudioManager.Instance.PlayCoin();

        Destroy(gameObject);
    }
}

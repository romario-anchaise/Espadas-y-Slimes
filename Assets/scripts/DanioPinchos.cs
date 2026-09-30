using UnityEngine;

public class DanioPinchos : MonoBehaviour
{
    public int cantidadDanio = 10;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Verifica si el objeto que acaba de tocar los pinchos tiene la etiqueta "Player"
        if (collision.CompareTag("Player"))
        {
            // Busca el script PlayerHealth en ese jugador
            PlayerHealth saludJugador = collision.GetComponent<PlayerHealth>();
            
            // Si encontró el script, ejecuta la función de restar vida
            if (saludJugador != null)
            {
                saludJugador.RecibirDanio(cantidadDanio);
            }
        }
    }
}

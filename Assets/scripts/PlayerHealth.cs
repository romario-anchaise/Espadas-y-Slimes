using System;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] int vidaMaxima = 100;
    int vida;

    public event Action Damaged;

    void Start()
    {
        // Al iniciar, la vida actual es igual a la máxima
        vida = vidaMaxima;
        
        // Llamamos al Singleton de UIManager para actualizar la barra al empezar
        if (UIManager.Instance != null)
            UIManager.Instance.ActualizarVida(vida, vidaMaxima);
    }

    public void RecibirDanio(int cantidad)
    {
        int vidaAnterior = vida;

        // Mathf.Max evita que la vida baje de 0 (si vida - cantidad es negativo, se queda en 0)
        vida = Mathf.Max(0, vida - cantidad);
        
        // Actualizamos la barra de vida en la UI cada vez que recibimos daño
        if (UIManager.Instance != null)
        {
            UIManager.Instance.ActualizarVida(vida, vidaMaxima);
            UIManager.Instance.FlashDanio();
        }

        if (vida < vidaAnterior)
            Damaged?.Invoke();
    }
}

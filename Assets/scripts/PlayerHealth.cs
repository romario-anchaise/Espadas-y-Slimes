using System;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] int vidaMaxima = 100;
    int vida;

    public int VidaActual => vida;
    public int VidaMaxima => vidaMaxima;

    public event Action<int, int> HealthChanged;
    public event Action Damaged;

    void Start()
    {
        vida = vidaMaxima;
        HealthChanged?.Invoke(vida, vidaMaxima);
    }

    public void RecibirDanio(int cantidad)
    {
        int vidaAnterior = vida;

        vida = Mathf.Max(0, vida - cantidad);

        if (vida < vidaAnterior)
        {
            HealthChanged?.Invoke(vida, vidaMaxima);
            Damaged?.Invoke();
        }
    }
}

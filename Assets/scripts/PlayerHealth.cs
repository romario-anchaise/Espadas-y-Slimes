using System;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] int vidaMaxima = 100;
    int vida;

    public int VidaActual => vida;
    public int VidaMaxima => vidaMaxima;
    public bool IsDead => vida <= 0;

    public event Action<int, int> HealthChanged;
    public event Action Damaged;
    public event Action Died;

    void Start()
    {
        vida = vidaMaxima;
        HealthChanged?.Invoke(vida, vidaMaxima);
    }

    public void RecibirDanio(int cantidad)
    {
        if (cantidad <= 0 || IsDead)
            return;

        int vidaAnterior = vida;

        vida = Mathf.Max(0, vida - cantidad);

        if (vida < vidaAnterior)
        {
            HealthChanged?.Invoke(vida, vidaMaxima);
            Damaged?.Invoke();

            if (IsDead)
                Died?.Invoke();
        }
    }
}

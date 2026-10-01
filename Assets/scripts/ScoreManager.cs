using System;
using UnityEngine;

public sealed class ScoreManager : MonoBehaviour
{
    private static ScoreManager s_instance;

    public static ScoreManager Instance
    {
        get
        {
            if (s_instance == null)
            {
                GameObject scoreObject = new GameObject("ScoreManager");
                s_instance = scoreObject.AddComponent<ScoreManager>();
            }

            return s_instance;
        }
    }

    public int CurrentScore { get; private set; }

    public event Action<int> ScoreChanged;
    public event Action<int> PointsAdded;

    private void Awake()
    {
        if (s_instance != null && s_instance != this)
        {
            Destroy(gameObject);
            return;
        }

        s_instance = this;
    }

    public void AddPoints(int amount)
    {
        if (amount <= 0)
            return;

        CurrentScore += amount;
        ScoreChanged?.Invoke(CurrentScore);
        PointsAdded?.Invoke(amount);
    }

    public static bool TryGetInstance(out ScoreManager scoreManager)
    {
        scoreManager = s_instance;
        return scoreManager != null;
    }
}

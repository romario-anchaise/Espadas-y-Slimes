using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerRespawn : MonoBehaviour
{
    [Header("Reinicio por caída")]
    [SerializeField, Min(1f)] private float m_fallDistance = 12f;

    private float m_startY;
    private bool m_restarting;

    private void Awake()
    {
        m_startY = transform.position.y;
    }

    private void Update()
    {
        if (!m_restarting && transform.position.y < m_startY - m_fallDistance)
            RestartLevel();
    }

    private void RestartLevel()
    {
        m_restarting = true;

        PlayerMovement movement = GetComponent<PlayerMovement>();
        if (movement != null)
            movement.enabled = false;

        Time.timeScale = 1f;
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.buildIndex);
    }
}

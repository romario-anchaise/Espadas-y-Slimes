using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerRespawn : MonoBehaviour
{
    [Header("Reinicio por caída")]
    [SerializeField, Min(1f)] private float m_fallDistance = 12f;

    [Header("Reinicio por muerte")]
    [SerializeField, Min(0.1f)] private float m_deathDelay = 1.25f;

    private float m_startY;
    private bool m_restarting;
    private PlayerHealth m_health;

    private void Awake()
    {
        m_startY = transform.position.y;
        m_health = GetComponent<PlayerHealth>();
    }

    private void OnEnable()
    {
        if (m_health != null)
            m_health.Died += HandleDeath;
    }

    private void OnDisable()
    {
        if (m_health != null)
            m_health.Died -= HandleDeath;
    }

    private void HandleDeath()
    {
        if (m_restarting)
            return;

        m_restarting = true;

        PlayerMovement movement = GetComponent<PlayerMovement>();
        if (movement != null)
            movement.enabled = false;

        PlayerCombat combat = GetComponent<PlayerCombat>();
        if (combat != null)
            combat.enabled = false;

        Rigidbody2D body = GetComponent<Rigidbody2D>();
        if (body != null)
        {
            body.linearVelocity = Vector2.zero;
            body.constraints = RigidbodyConstraints2D.FreezeAll;
        }

        StartCoroutine(RestartAfterDeath());
    }

    private IEnumerator RestartAfterDeath()
    {
        yield return new WaitForSecondsRealtime(m_deathDelay);
        LoadCurrentScene();
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

        LoadCurrentScene();
    }

    private static void LoadCurrentScene()
    {
        Time.timeScale = 1f;
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.buildIndex);
    }
}

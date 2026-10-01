using UnityEngine;

[RequireComponent(typeof(PlayerMovement), typeof(PlayerHealth))]
public class PlayerAudio : MonoBehaviour
{
    private PlayerMovement m_movement;
    private PlayerHealth m_health;

    private void Awake()
    {
        m_movement = GetComponent<PlayerMovement>();
        m_health = GetComponent<PlayerHealth>();
    }

    private void OnEnable()
    {
        if (m_movement != null)
            m_movement.Jumped += PlayJump;
        if (m_health != null)
            m_health.Damaged += PlayDamage;
    }

    private void OnDisable()
    {
        if (m_movement != null)
            m_movement.Jumped -= PlayJump;
        if (m_health != null)
            m_health.Damaged -= PlayDamage;
    }

    private void PlayJump()
    {
        AudioManager.Instance.PlayJump();
    }

    private void PlayDamage()
    {
        AudioManager.Instance.PlayDamage();
    }
}

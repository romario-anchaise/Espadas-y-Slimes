using UnityEngine;

[RequireComponent(typeof(PlayerMovement), typeof(PlayerHealth))]
public class PlayerVfx : MonoBehaviour
{
    private PlayerMovement movement;
    private PlayerHealth health;

    private void Awake()
    {
        movement = GetComponent<PlayerMovement>();
        health = GetComponent<PlayerHealth>();
    }

    private void OnEnable()
    {
        movement.Jumped += PlayJumpParticles;
        health.Damaged += PlayDamageParticles;
    }

    private void OnDisable()
    {
        movement.Jumped -= PlayJumpParticles;
        health.Damaged -= PlayDamageParticles;
    }

    private void PlayJumpParticles()
    {
        ParticleEffects.PlayJump(transform.position + Vector3.down * 0.55f);
    }

    private void PlayDamageParticles()
    {
        ParticleEffects.PlayDamage(transform.position + Vector3.up * 0.5f);
    }
}

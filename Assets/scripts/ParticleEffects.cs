using UnityEngine;

public static class ParticleEffects
{
    public static void PlayJump(Vector3 position)
    {
        SpawnBurst("JumpParticles", position, new Color(0.82f, 0.76f, 0.62f), 10, 0.18f, 1.5f);
    }

    public static void PlayDamage(Vector3 position)
    {
        SpawnBurst("DamageParticles", position, new Color(0.95f, 0.15f, 0.12f), 14, 0.12f, 2.2f);
    }

    public static void PlayCoin(Vector3 position)
    {
        SpawnBurst("CoinParticles", position, new Color(1f, 0.78f, 0.12f), 16, 0.1f, 2f);
    }

    private static void SpawnBurst(string effectName, Vector3 position, Color color, int count, float size, float speed)
    {
        GameObject effect = new GameObject(effectName);
        effect.transform.position = position;

        ParticleSystem particles = effect.AddComponent<ParticleSystem>();
        ParticleSystem.MainModule main = particles.main;
        main.duration = 0.35f;
        main.loop = false;
        main.startLifetime = 0.45f;
        main.startSpeed = speed;
        main.startSize = size;
        main.startColor = color;
        main.simulationSpace = ParticleSystemSimulationSpace.World;
        main.stopAction = ParticleSystemStopAction.Destroy;

        ParticleSystem.EmissionModule emission = particles.emission;
        emission.rateOverTime = 0f;
        emission.SetBursts(new[] { new ParticleSystem.Burst(0f, (short)count) });

        ParticleSystem.ShapeModule shape = particles.shape;
        shape.shapeType = ParticleSystemShapeType.Circle;
        shape.radius = 0.18f;

        ParticleSystem.ColorOverLifetimeModule colorOverLifetime = particles.colorOverLifetime;
        colorOverLifetime.enabled = true;
        Gradient fade = new Gradient();
        fade.SetKeys(
            new[] { new GradientColorKey(color, 0f), new GradientColorKey(color, 1f) },
            new[] { new GradientAlphaKey(1f, 0f), new GradientAlphaKey(0f, 1f) });
        colorOverLifetime.color = fade;

        particles.Play();
    }
}

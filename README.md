# Espadas y Slimes

Juego de plataformas 2D desarrollado en Unity 6.3 LTS. El jugador recorre un nivel construido con Tilemap, recoge puntos, evita obstáculos y recibe feedback visual y sonoro.

## Controles

- `A/D` o flechas: movimiento.
- `Espacio`: salto.
- `J`, clic izquierdo o botón oeste del mando: ataque.
- `Escape`: abrir o cerrar el menú de pausa.

## Arquitectura

- `PlayerMovement`: entrada de movimiento, salto y física.
- `PlayerCombat`: entrada y temporización del ataque.
- `PlayerAnimationController`: traduce eventos y estado del jugador al Animator.
- `PlayerHealth`: administra la vida y publica `HealthChanged` y `Damaged`.
- `ScoreManager`: singleton que administra el puntaje y publica eventos.
- `UIManager`: escucha los eventos de vida y puntaje; no consulta esos sistemas en `Update`.
- `AudioManager`: singleton persistente con canales `Music` y `SFX` en un Audio Mixer.
- `PlayerAudio`: reproduce sonidos al escuchar eventos del jugador.
- `PlayerVfx` y `ParticleEffects`: partículas de salto, daño y recolección.
- `PlayerRespawn`: reinicia el nivel cuando el personaje cae.

## Evidencias de la rúbrica

- Nivel 2D con Tilemap, Composite Collider y cámara Cinemachine.
- Animator con estados Idle, Run, Jump, Fall, Attack, Hurt y Death.
- Menú principal, HUD reactivo y menú de pausa.
- SFX, música de fondo, Audio Mixer y partículas coordinadas con acciones.
- Código modular con managers singleton y comunicación mediante eventos C#.

## Entrega

El ejecutable comprimido se genera en `Entregables/Espadas-y-Slimes-Windows.zip`. Para la entrega grupal todavía se debe adjuntar el enlace al video de sustentación y verificar que aparezcan commits de todos los integrantes en GitHub.

Los efectos de sonido externos y sus licencias están documentados en `Assets/Resources/Audio/LICENSES.md`.

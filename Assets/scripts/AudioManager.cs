using UnityEngine;
using UnityEngine.Audio;

public sealed class AudioManager : MonoBehaviour
{
    private static AudioManager s_instance;

    private AudioSource m_source;
    private AudioSource m_musicSource;
    private AudioClip m_jumpClip;
    private AudioClip m_damageClip;
    private AudioClip m_coinClip;
    private AudioClip m_buttonClip;

    public static AudioManager Instance
    {
        get
        {
            if (s_instance == null)
            {
                GameObject audioObject = new GameObject("AudioManager");
                s_instance = audioObject.AddComponent<AudioManager>();
            }

            return s_instance;
        }
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void InitializeBeforeScene()
    {
        _ = Instance;
    }

    private void Awake()
    {
        if (s_instance != null && s_instance != this)
        {
            Destroy(gameObject);
            return;
        }

        s_instance = this;
        DontDestroyOnLoad(gameObject);

        m_source = gameObject.AddComponent<AudioSource>();
        m_source.playOnAwake = false;
        m_source.spatialBlend = 0f;

        AudioMixer mixer = Resources.Load<AudioMixer>("Audio/GameAudioMixer");
        if (mixer != null)
        {
            AudioMixerGroup[] sfxGroups = mixer.FindMatchingGroups("SFX");
            if (sfxGroups.Length > 0)
                m_source.outputAudioMixerGroup = sfxGroups[0];
        }

        m_jumpClip = Resources.Load<AudioClip>("Audio/player_jump");
        m_damageClip = Resources.Load<AudioClip>("Audio/player_hurt");
        m_coinClip = Resources.Load<AudioClip>("Audio/coin_pickup");
        m_buttonClip = Resources.Load<AudioClip>("Audio/ui_button");

        StartBackgroundMusic(mixer);
    }

    private void StartBackgroundMusic(AudioMixer mixer)
    {
        m_musicSource = gameObject.AddComponent<AudioSource>();
        m_musicSource.playOnAwake = false;
        m_musicSource.loop = true;
        m_musicSource.spatialBlend = 0f;
        m_musicSource.volume = 0.18f;

        if (mixer != null)
        {
            AudioMixerGroup[] musicGroups = mixer.FindMatchingGroups("Music");
            if (musicGroups.Length > 0)
                m_musicSource.outputAudioMixerGroup = musicGroups[0];
        }

        m_musicSource.clip = CreateBackgroundLoop();
        m_musicSource.Play();
    }

    private static AudioClip CreateBackgroundLoop()
    {
        const int sampleRate = 22050;
        const int seconds = 8;
        float[] notes = { 220f, 261.63f, 329.63f, 293.66f, 196f, 246.94f, 293.66f, 261.63f };
        float[] samples = new float[sampleRate * seconds];

        for (int i = 0; i < samples.Length; i++)
        {
            float time = (float)i / sampleRate;
            int noteIndex = Mathf.FloorToInt(time * 2f) % notes.Length;
            float beatTime = (time * 2f) % 1f;
            float envelope = Mathf.SmoothStep(1f, 0f, beatTime);
            float melody = Mathf.Sin(2f * Mathf.PI * notes[noteIndex] * time);
            float bass = Mathf.Sin(2f * Mathf.PI * (notes[noteIndex] * 0.5f) * time);
            samples[i] = (melody * 0.08f + bass * 0.05f) * envelope;
        }

        AudioClip clip = AudioClip.Create("EspadasYSlimes_BGM", samples.Length, 1, sampleRate, false);
        clip.SetData(samples, 0);
        return clip;
    }

    public void PlayJump()
    {
        Play(m_jumpClip, 0.55f);
    }

    public void PlayDamage()
    {
        Play(m_damageClip, 0.65f);
    }

    public void PlayCoin()
    {
        Play(m_coinClip, 0.7f);
    }

    public void PlayButton()
    {
        Play(m_buttonClip, 0.55f);
    }

    private void Play(AudioClip clip, float volume)
    {
        if (clip != null && m_source != null)
            m_source.PlayOneShot(clip, volume);
    }
}

using UnityEngine;

public sealed class AudioManager : MonoBehaviour
{
    private static AudioManager s_instance;

    private AudioSource m_source;
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

        m_jumpClip = Resources.Load<AudioClip>("Audio/player_jump");
        m_damageClip = Resources.Load<AudioClip>("Audio/player_hurt");
        m_coinClip = Resources.Load<AudioClip>("Audio/coin_pickup");
        m_buttonClip = Resources.Load<AudioClip>("Audio/ui_button");
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

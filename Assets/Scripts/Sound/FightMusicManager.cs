using UnityEngine;

public class FightMusicManager : MonoBehaviour
{
    public AudioClip[]  Musics; //Clips are stored in here.
    private AudioSource m_FightAudioSrc;
    private float       m_StartVolume;
    private bool        m_IsMusicActive;
    private bool        m_HasEverPlayedBefore;
    void Start()
    {
        m_FightAudioSrc = GetComponent<AudioSource>();
        m_StartVolume = m_FightAudioSrc.volume;
        EventManager.GetInstance().OnTriggerFightMusicFirstTime += OnTriggerForTheFirstTime;
        EventManager.GetInstance().OnLockTargetDeath += OnEnemyDeath;
        EventManager.GetInstance().OnEnemySeesPlayer += OnEnemySeesPlayer;
        EventManager.GetInstance().OnPlayerClosesTutorial += OnPlayerClosesTutorial;
        m_IsMusicActive = false;
        m_HasEverPlayedBefore = false;
    }
    void OnTriggerForTheFirstTime() //Triggered when the playe enters the tutorial screen. The slow paced music plays here to avoid distracting the playe while reading.
    {
        m_HasEverPlayedBefore = true;
        m_FightAudioSrc.clip = Musics[0];
        m_FightAudioSrc.Play();
        m_IsMusicActive = true;
    }
    void OnEnemyDeath() 
    {
        FadeOut();
        m_IsMusicActive = false;
    }
    void OnEnemySeesPlayer() 
    {
        if(!m_IsMusicActive)
        {
            if (!m_HasEverPlayedBefore)
            {
                m_FightAudioSrc.clip = Musics[1];
                m_FightAudioSrc.Play();
                m_HasEverPlayedBefore = true;
            }
            FadeIn();
        }
    }
    void OnPlayerClosesTutorial()
    {
        FadeOut(0.7f);
        m_IsMusicActive = false;
        m_HasEverPlayedBefore = false;
    }

    void FadeIn(float time = 0.5f)
    {
        StartCoroutine(SoundManager.StartFade(m_FightAudioSrc, time, m_StartVolume));
    }
    void FadeOut(float time = 3.0f)
    {
        StartCoroutine(SoundManager.StartFade(m_FightAudioSrc, time, 0));
    }

}

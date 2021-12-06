using UnityEngine;
using UnityEngine.SceneManagement;
public class WindSound : MonoBehaviour
{
    private AudioSource m_WindAudioSource;
    private float m_StartVolume;

    void Start()
    {
        m_WindAudioSource = GetComponent<AudioSource>();
        m_StartVolume = m_WindAudioSource.volume;
        if(SceneManager.GetActiveScene().buildIndex == 1)
        {
            EventManager.GetInstance().OnPlayerDisablesWindSound += FadeOut;
            EventManager.GetInstance().OnPlayerEnablesWindSound += FadeIn;
        }
        m_WindAudioSource.volume = 0;
        if(GameState.WasTutorialAlreadyTriggered || SceneManager.GetActiveScene().buildIndex == 0)
        {
            FadeIn();
        }
    }
    public void FadeOut()
    {
        StartCoroutine(SoundManager.StartFade(m_WindAudioSource, 1, 0));
    }
    public void FadeIn()
    {
        StartCoroutine(SoundManager.StartFade(m_WindAudioSource, 1, m_StartVolume));
    }
}

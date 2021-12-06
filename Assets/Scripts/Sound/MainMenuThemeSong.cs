using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
public class MainMenuThemeSong : MonoBehaviour
{
    private AudioSource m_ThemeSource;
    private float       m_DelayTime;
    void Start()
    {
        m_DelayTime = 2;
        EventManager.GetInstance().OnPlayButtonPressed += OnPlayButtonPressed;
        m_ThemeSource = GetComponent<AudioSource>();
        m_ThemeSource.volume = 0;
        if (SceneManager.GetActiveScene().buildIndex == 0)
        {
            StartCoroutine(Delay());
        }
    }
    IEnumerator Delay()
    {
        while(m_DelayTime >= 0.0f)
        {
            m_DelayTime -= Time.deltaTime;
            yield return null;
        }
        m_ThemeSource.volume = 1;
        m_ThemeSource.Play();
        yield return null;
    }
    private void OnPlayButtonPressed()
    {
        StopAllCoroutines();
        StartCoroutine(SoundManager.StartFade(m_ThemeSource, 0.5f, 0));
    }
}

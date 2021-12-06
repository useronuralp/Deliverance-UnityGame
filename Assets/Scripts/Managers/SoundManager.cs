using System.Collections;
using UnityEngine;
//Responsible for managing SFXs in the game. THIS DOES NOT HANDLE MUSIC.
public class SoundManager : MonoBehaviour
{
    private static AudioClip m_GetHit1, m_GetHit2, m_GetHit3, m_GetHit4, m_MissSound;
    private static AudioClip m_BlockSound1, m_BlockSound2, m_BlockSound3, m_BlockSound4, m_BlockSound5, m_BlockSound6, m_BlockSound7, m_BlockSound8;
    public static AudioSource m_AudioSrcHitSounds;
    private static AudioClip m_ComboSuccessSound;
    void Awake()
    {
        m_ComboSuccessSound = Resources.Load<AudioClip>("ComboSuccess");
        m_GetHit1 =           Resources.Load<AudioClip>("skin1");
        m_GetHit2 =           Resources.Load<AudioClip>("GetHitBody");
        m_GetHit3 =           Resources.Load<AudioClip>("skin2");
        m_GetHit4 =           Resources.Load<AudioClip>("skin4");
        m_MissSound =         Resources.Load<AudioClip>("Miss");
        m_BlockSound1 =       Resources.Load<AudioClip>("Hit1");
        m_BlockSound2 =       Resources.Load<AudioClip>("Hit2");
        m_BlockSound3 =       Resources.Load<AudioClip>("Hit3");
        m_BlockSound4 =       Resources.Load<AudioClip>("Hit4");
        m_BlockSound5 =       Resources.Load<AudioClip>("Hit5");
        m_BlockSound6 =       Resources.Load<AudioClip>("Hit6");
        m_BlockSound7 =       Resources.Load<AudioClip>("Hit7");
        m_BlockSound8 =       Resources.Load<AudioClip>("Hit8");

        m_AudioSrcHitSounds = GetComponent<AudioSource>();
    }
    public static void PlaySound(string clipName, float volume = 1.0f)
    {
        m_AudioSrcHitSounds.volume = volume;
        switch(clipName)
        {
            case "GetHit1": m_AudioSrcHitSounds.volume = 0.5f; m_AudioSrcHitSounds.PlayOneShot(m_GetHit1);      break;
            case "GetHit2": m_AudioSrcHitSounds.PlayOneShot(m_GetHit2);     break;
            case "GetHit3": m_AudioSrcHitSounds.volume = 0.5f;  m_AudioSrcHitSounds.PlayOneShot(m_GetHit3); break;
            case "GetHit4": m_AudioSrcHitSounds.PlayOneShot(m_GetHit4);     break;
            case "Miss":    m_AudioSrcHitSounds.PlayOneShot(m_MissSound);   break;
            case "Block1":  m_AudioSrcHitSounds.PlayOneShot(m_BlockSound1); break;
            case "Block2":  m_AudioSrcHitSounds.PlayOneShot(m_BlockSound2); break;
            case "Block3":  m_AudioSrcHitSounds.PlayOneShot(m_BlockSound3); break;
            case "Block4":  m_AudioSrcHitSounds.PlayOneShot(m_BlockSound4); break;
            case "Block5":  m_AudioSrcHitSounds.PlayOneShot(m_BlockSound5); break;
            case "Block6":  m_AudioSrcHitSounds.PlayOneShot(m_BlockSound6); break;
            case "Block7":  m_AudioSrcHitSounds.PlayOneShot(m_BlockSound7); break;
            case "Block8":  m_AudioSrcHitSounds.PlayOneShot(m_BlockSound8); break;
            case "ComboSuccess": m_AudioSrcHitSounds.PlayOneShot(m_ComboSuccessSound); break;
        }
    }
    public static AudioSource GetAudioSource()
    {
        return m_AudioSrcHitSounds;
    }
    public static IEnumerator StartFade(AudioSource audioSource, float duration, float targetVolume) //This fnc is useful for slowly fading in / out a music track.
    {
        float currentTime = 0;
        float start = audioSource.volume;

        while (currentTime < duration)
        {
            currentTime += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(start, targetVolume, currentTime / duration);
            yield return null;
        }
        yield break;
    }
}

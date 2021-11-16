using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    private static AudioClip m_GetHitSound, m_GetHitBodySound, m_GetHitPunchSound, m_MissSound;
    private static AudioClip m_BlockSound1, m_BlockSound2, m_BlockSound3;
    private static AudioSource m_AudioSrc;
    void Awake()
    {
        m_GetHitSound =      Resources.Load<AudioClip>("GetHit");
        m_GetHitBodySound =  Resources.Load<AudioClip>("GetHitBody");
        m_GetHitPunchSound = Resources.Load<AudioClip>("GetHitPunch");
        m_MissSound =        Resources.Load<AudioClip>("Miss");
        m_BlockSound1 =      Resources.Load<AudioClip>("Block1");
        m_BlockSound2 =      Resources.Load<AudioClip>("Block2");
        m_BlockSound3 =      Resources.Load<AudioClip>("Block3");

        m_AudioSrc = GetComponent<AudioSource>();
    }

    public static void PlaySound(string clipName)
    {
        m_AudioSrc.volume = 0.3f;
        switch(clipName)
        {
            case "GetHit":      m_AudioSrc.PlayOneShot(m_GetHitSound);      break;
            case "GetHitBody":  m_AudioSrc.PlayOneShot(m_GetHitBodySound);  break;
            case "GetHitPunch": m_AudioSrc.PlayOneShot(m_GetHitPunchSound); break;
            case "Miss":        m_AudioSrc.PlayOneShot(m_MissSound);        break;

            case "Block1":      m_AudioSrc.PlayOneShot(m_BlockSound1);      break;
            case "Block2":      m_AudioSrc.PlayOneShot(m_BlockSound2);      break;
            case "Block3":      m_AudioSrc.PlayOneShot(m_BlockSound3);      break;
        }
    }
    public static AudioSource GetAudioSource()
    {
        return m_AudioSrc;
    }
}

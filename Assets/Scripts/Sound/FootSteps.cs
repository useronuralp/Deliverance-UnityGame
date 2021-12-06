using UnityEngine;

//The class that plays the footstep sounds in the game.
public class FootSteps : MonoBehaviour
{
    public AudioSource        m_AudioSource;
    public AudioClip[]        GrassClips;
    public AudioClip[]        RockClips;
    private MovementBehaviour m_MovementScript;
    private void Start()
    {
        m_MovementScript = GetComponent<MovementBehaviour>();
        m_AudioSource = GetComponent<AudioSource>();
    }
    public void Step()
    {
        if(gameObject.CompareTag("Rescuer"))
        {
            AudioClip clip = GrassClips[Random.Range(0, GrassClips.Length)];
            m_AudioSource.Stop();
            m_AudioSource.PlayOneShot(clip);
            return;
        }
        if(m_MovementScript.m_IsOnGrass)
        {
            AudioClip clip = GrassClips[Random.Range(0, GrassClips.Length)];
            m_AudioSource.Stop();
            m_AudioSource.PlayOneShot(clip);
        }
        else
        {
            AudioClip clip = RockClips[Random.Range(0, RockClips.Length)];
            m_AudioSource.Stop();
            m_AudioSource.PlayOneShot(clip);
        }
    }
}

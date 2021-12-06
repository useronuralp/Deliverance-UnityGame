using UnityEngine;
using UnityEngine.UI;
//Shows the 'E' image when player gets near Rescuer and the locked gate.
public class PressETrigger : MonoBehaviour
{
    private Transform m_PressE;
    void Start()
    {
        EventManager.GetInstance().OnPlayerStepsIntoTalkRange += OnPlayerEntersTalkRange;
        EventManager.GetInstance().OnPlayerLeavingTalkRange += OnPlayerLeavesTalkRange;
        EventManager.GetInstance().OnPlayerStartsSpeakingWithRescuer += OnPlayerStartsSpeakingWithRescuer;
        EventManager.GetInstance().OnPlayerEntersDoorUnlockRange += OnPlayerEntersDoorUnlockRange;
        EventManager.GetInstance().OnPlayerLeavesDoorUnlockRange += OnPlayerLeavesDoorUnlockRange;
        m_PressE = transform.Find("E");
    }
    void OnPlayerEntersTalkRange()
    {
        m_PressE.GetComponent<Image>().enabled = true;
    }
    void OnPlayerLeavesTalkRange()
    {
        m_PressE.GetComponent<Image>().enabled = false;
    }
    void OnPlayerStartsSpeakingWithRescuer()
    {
        m_PressE.GetComponent<Image>().enabled = false;
    }
    void OnPlayerEntersDoorUnlockRange()
    {
        m_PressE.GetComponent<Image>().enabled = true;
    }
    void OnPlayerLeavesDoorUnlockRange()
    {
        m_PressE.GetComponent<Image>().enabled = false;
    }
}

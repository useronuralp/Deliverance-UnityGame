using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class RescuerTalkEvent : MonoBehaviour
{
    // Start is called before the first frame update
    private Transform m_PressE;
    void Start()
    {
        EventManager.GetInstance().OnPlayerStepsIntoTalkRange += OnPlayerEntersTalkRange;
        EventManager.GetInstance().OnPlayerLeavingTalkRange += OnPlayerLeavesTalkRange;
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
}

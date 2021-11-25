using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorTrigger : MonoBehaviour
{
    private bool m_IsPlayerUnlockRange;
    private void Start()
    {
        m_IsPlayerUnlockRange = false;
        EventManager.GetInstance().OnPlayerCompletingDialogue += OnPlayerCompletingDialogue;
    }
    private void Update()
    {
        if(m_IsPlayerUnlockRange)
        {
            if(Input.GetKeyDown(KeyCode.E))
            {
                transform.root.Find("DoorLock").GetComponent<BoxCollider>().enabled = false;
                EventManager.GetInstance().PlayerLeftDoorUnlockRange();
                Destroy(gameObject);
            }
        }
    }
    void OnPlayerCompletingDialogue()
    {
        GetComponent<BoxCollider>().enabled = true;
    }
    private void OnTriggerEnter(Collider other)
    {
        m_IsPlayerUnlockRange = true;
        EventManager.GetInstance().PlayerEnteredDoorUnlockRange();
    }
    private void OnTriggerExit(Collider other)
    {
        m_IsPlayerUnlockRange = false;
        EventManager.GetInstance().PlayerLeftDoorUnlockRange();
    }
}

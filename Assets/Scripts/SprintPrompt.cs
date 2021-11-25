using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SprintPrompt : MonoBehaviour
{
    private bool m_PlayerSawPrompt;
    void Start()
    {
        m_PlayerSawPrompt = false;
        EventManager.GetInstance().OnPlayerEnteredSprintTrigger += OnPlayerEnteredSprintTrigger;
    }

    void Update()
    {
        if(m_PlayerSawPrompt)
        {
            if(Input.GetKeyDown(KeyCode.LeftShift))
            {
                Destroy(gameObject);
            }
        }
    }
    void OnPlayerEnteredSprintTrigger()
    {
        m_PlayerSawPrompt = true;
        transform.Find("Prompt").gameObject.SetActive(true);
    }
}

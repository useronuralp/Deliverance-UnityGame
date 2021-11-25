using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueBox : MonoBehaviour
{
    private void Start()
    {
        EventManager.GetInstance().OnPlayerLeavingTalkRange += OnPlayerLeavingRange;
        EventManager.GetInstance().OnPlayerCompletingDialogue += OnPlayerCompletingDialogue;
    }
    void OnPlayerLeavingRange()
    {
        gameObject.SetActive(false);
    }
    void OnPlayerCompletingDialogue()
    {
        Destroy(gameObject);
    }
}

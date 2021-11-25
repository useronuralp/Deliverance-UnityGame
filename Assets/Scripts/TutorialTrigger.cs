using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialTrigger : MonoBehaviour
{
    private void Start()
    {
        EventManager.GetInstance().OnPlayerClosesTutorial += OnPlayerCompletesTutorial;
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            EventManager.GetInstance().PlayerEnteredTutorialTrigger();
        }
    }
    void OnPlayerCompletesTutorial()
    {
        Destroy(gameObject);
    }
}

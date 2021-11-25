using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StopTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        EventManager.GetInstance().RescuerEnteredStopTrigger();
        Destroy(gameObject);
    }
}

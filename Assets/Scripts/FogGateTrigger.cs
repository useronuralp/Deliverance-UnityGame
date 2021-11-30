using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FogGateTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            switch(gameObject.name)
            {
                case "FogGateTrigger":  EventManager.GetInstance().LockArea1(); break;
                case "FogGateTrigger2": EventManager.GetInstance().LockArea2(); break;
                case "FogGateTrigger3": EventManager.GetInstance().LockArea3(); break;
            }
            Destroy(gameObject);
        }
    }
}

using UnityEngine;
//This class triggers the red fog gates in the game to lock down when player enters the invisible trigger.
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

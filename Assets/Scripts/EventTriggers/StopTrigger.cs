using UnityEngine;
//Fires the event to signal the rescuer to face towards the window to speak to the player.
public class StopTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        EventManager.GetInstance().RescuerEnteredStopTrigger();
        Destroy(gameObject);
    }
}

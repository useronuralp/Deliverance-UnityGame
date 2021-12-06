using UnityEngine;
//Fires the event that triggers the 'Press CTRL to sprint' text on screen.
public class SprintTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            EventManager.GetInstance().PlayerEnteredSprintTrigger();
            Destroy(gameObject);
        }
    }
}

using UnityEngine;
//Fires the events of enabling/disabling wind sound in the game. In closed spaces the sound gets disabled.
public class WindStartStopTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            if(name.Contains("Stop"))
            {
                EventManager.GetInstance().DisableWindSound();
            }
            else if(name.Contains("Start"))
            {
                EventManager.GetInstance().EnableWindSound();
            }
        }
    }
}

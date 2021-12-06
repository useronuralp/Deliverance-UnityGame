using UnityEngine;
//Fires the event to disable leaf particles when player goes back into closed space.
public class LeafParticleDisabler : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            EventManager.GetInstance().DisableParticles();
        }
    }
}

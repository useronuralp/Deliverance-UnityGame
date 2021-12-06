using UnityEngine;
//This class fires the event when player leaves the closed space, so that the leaf particles can start flying.
public class LeafParticleEnabler : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        EventManager.GetInstance().EnableParticles();
    }
}

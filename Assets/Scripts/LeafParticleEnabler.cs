using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeafParticleEnabler : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        EventManager.GetInstance().EnableParticles();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

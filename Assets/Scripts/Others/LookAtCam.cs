using UnityEngine;
//Simple script used to rotate the health/stamina bar over the enemy to look at the camera.
public class LookAtCam : MonoBehaviour
{
    void Update()
    {
        Vector3 lookAt = transform.position - Camera.main.transform.position;
        lookAt.x = lookAt.z = 0.0f;
        transform.LookAt(Camera.main.transform.position - lookAt);
    }
}

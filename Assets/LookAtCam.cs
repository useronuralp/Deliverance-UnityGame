using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LookAtCam : MonoBehaviour
{
    void Update()
    {
        Vector3 lookAt = transform.position - Camera.main.transform.position;
        lookAt.x = lookAt.z = 0.0f;
        transform.LookAt(Camera.main.transform.position - lookAt);
    }
}

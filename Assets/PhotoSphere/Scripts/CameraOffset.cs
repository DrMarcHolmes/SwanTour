using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraOffset : MonoBehaviour
{
    public float cameraHeightOffset;

    void Start()
    {
        transform.position = new Vector3(transform.position.x, transform.position.y + cameraHeightOffset, transform.position.z);
    }
}
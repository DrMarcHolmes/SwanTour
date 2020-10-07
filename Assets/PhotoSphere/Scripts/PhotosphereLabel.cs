using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[ExecuteInEditMode]
public class PhotosphereLabel : MonoBehaviour
{
    private void Update()
    {
        if (Application.isPlaying == false)
        {
            // set label canvas to look at the centre of the photosphere
            transform.GetChild(0).transform.LookAt(transform.parent.parent);
        }
    }
}
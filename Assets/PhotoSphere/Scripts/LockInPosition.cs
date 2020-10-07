using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class LockInPosition : MonoBehaviour
{
    #if UNITY_EDITOR
    private void Update()
    {
        if(!Application.isPlaying)
        {
            if(transform.position != new Vector3(0,0,0))
            {
                Debug.LogWarning("The PhotoGimbal's position cannot be changed, move the parent photosphere object instead");
                transform.position = new Vector3(0,0,0);
                transform.localPosition = new Vector3(0,0,0);
            }
        }

    }
    #endif

}
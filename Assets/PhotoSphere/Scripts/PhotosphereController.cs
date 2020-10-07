using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhotosphereController : MonoBehaviour
{
    public Photosphere startSphere;
    public List<Photosphere> photospheres = new List<Photosphere>();
    private GameObject _controller;

    public void ConfigurePhotospheres(GameObject controller)
    {
        _controller = controller;        

        for (int i = 0; i < photospheres.Count; i++)
        {
            photospheres[i].ConfigurePhotosphere(_controller);

            if (photospheres[i] != startSphere)
                photospheres[i].PhotoGimbalEnabled = false;
            else
            {
                photospheres[i].HasBeenVisited = true;
            }
        }

        if (startSphere != null)
        {
            _controller.transform.position = startSphere.transform.position;

            if (startSphere.IsVideoSphere)
                startSphere.PlayVideo = true;
        }
        else
            Debug.LogError("No start Photosphere found, please check you have set up Photospheres correctly.");
    }

    public Transform FindPhotosphereTransform(string destinationSphereName)
    {
        RemoveMissingPhotospheres();

        for (int i = 0; i < photospheres.Count; i++)
        {
            if (photospheres[i].gameObject.name == destinationSphereName)
            {
                return photospheres[i].transform;
            }
        }

        Debug.LogWarning("No Photosphere destination found");
        return null;
    }

    public void AddPhotosphere(Photosphere sphere)
    {
        if (photospheres.Contains(sphere) == false)
            photospheres.Add(sphere);
    }

    // remove any missing photospheres after a user has made deletions
    public void RemoveMissingPhotospheres()
    {
        photospheres.RemoveAll(Photosphere => Photosphere == null);

        if (startSphere == null && photospheres.Count > 0)
            startSphere = photospheres[0];
    }
}
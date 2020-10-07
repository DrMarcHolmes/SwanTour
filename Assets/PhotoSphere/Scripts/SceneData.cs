using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class SceneData : MonoBehaviour
{
    private SceneProgression _sceneProgression;
    private PhotosphereController _photoSphereController;

    public float photoSpheresExplored;
    public float percentageOfSceneExplored;

    public bool allowSceneSelection;

    private void OnEnable()
    {
        Photosphere.OnVisited += RecalcutePercentageExplored;
    }

    private void Start()
    {
        _sceneProgression = GameObject.FindObjectOfType<SceneProgression>();
        _photoSphereController = GameObject.FindObjectOfType<PhotosphereController>();
    }

    public void RecalcutePercentageExplored()
    {
        if (_photoSphereController == null)
            _photoSphereController = GameObject.FindObjectOfType<PhotosphereController>();

		photoSpheresExplored = 0f;

        for (int i = 0; i < _photoSphereController.photospheres.Count; i++)
        {
            if (_photoSphereController.photospheres[i].HasBeenVisited)
            {
                photoSpheresExplored++;
            }
        }

        percentageOfSceneExplored = 100f /_photoSphereController.photospheres.Count;       
        percentageOfSceneExplored = percentageOfSceneExplored * photoSpheresExplored;
        percentageOfSceneExplored = (float)Math.Round(percentageOfSceneExplored);
    }

    private void OnDisable()
    {
        Photosphere.OnVisited -= RecalcutePercentageExplored;
    }
}
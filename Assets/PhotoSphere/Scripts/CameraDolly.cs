using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraDolly : MonoBehaviour
{
    public float rotateTime;
    private Transform _compass;
    public Camera dollyCamera;
    public LayerMask normalMask;
    public LayerMask sceneSelectionMask;

    public bool allowSceneSelection;
    private GameObject sceneSelectionUI;
    private bool _inSceneSelection;

    private void Awake()
    {
        _compass = transform.Find("Simple Compass");
    }

    public void RotateDolly(float rotationAmount)
    {
        StopAllCoroutines();
        StartCoroutine(Rotation(transform, new Vector3(0, rotationAmount, 0)));
    }

    public IEnumerator Rotation(Transform thisTransform, Vector3 degrees)
    {
        _compass.transform.SetParent(null);

        Quaternion startRotation = thisTransform.rotation;
        Quaternion endRotation = thisTransform.rotation * Quaternion.Euler(degrees);
        float rate = 1.0f / rotateTime;
        float t = 0.0f;
        while (t < 1.0f)
        {
            t += Time.deltaTime * rate;
            thisTransform.rotation = Quaternion.Slerp(startRotation, endRotation, t);
            yield return null;
        }

        _compass.SetParent(transform);
    }

    public void ToggleSceneSelectionCamera()
    {
        if (!allowSceneSelection)
            return;

        if (sceneSelectionUI == null)
        {
            Debug.LogError("VRUI not found in scene");
            return;
        }

        if (!_inSceneSelection)
        {
            sceneSelectionUI.transform.parent = transform;
            sceneSelectionUI.transform.localPosition = Vector3.zero;
            sceneSelectionUI.transform.eulerAngles = new Vector3(0, dollyCamera.transform.eulerAngles.y, 0);

            _inSceneSelection = true;

            dollyCamera.cullingMask = sceneSelectionMask;
        }
        else
        {
            _inSceneSelection = false;
            dollyCamera.cullingMask = normalMask;
        }

        sceneSelectionUI.SetActive(_inSceneSelection);
    }
}

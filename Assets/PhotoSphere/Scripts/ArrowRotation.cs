using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRStandardAssets.Utils;
public class ArrowRotation : MonoBehaviour
{
    public int rotationAmount;
    private CameraDolly _cameraDolly;
    public Material normalMaterial;
    public Material overMaterial;
    public Material clickedMaterial;
    public Material doubleClickedMaterial;
    private VRInteractiveItem _VRInteractiveItem;
    private MeshRenderer _meshRenderer;

    private void Awake()
    {
        _meshRenderer = GetComponent<MeshRenderer>();
        _VRInteractiveItem = GetComponent<VRInteractiveItem>();
        _cameraDolly = transform.root.GetComponent<CameraDolly>();       
    }

    private void OnEnable()
    {
        _VRInteractiveItem.OnOver += HandleOver;
        _VRInteractiveItem.OnOut += HandleOut;
        _VRInteractiveItem.OnClick += HandleClick;
    }


    private void OnDisable()
    {
        _VRInteractiveItem.OnOver -= HandleOver;
        _VRInteractiveItem.OnOut -= HandleOut;
        _VRInteractiveItem.OnClick -= HandleClick;
    }


    //Handle the Over event
    private void HandleOver()
    {
        // Debug.Log("Show over state");
        _meshRenderer.material = overMaterial;
    }


    //Handle the Out event
    private void HandleOut()
    {
        // Debug.Log("Show out state");
        _meshRenderer.material = normalMaterial;
    }


    //Handle the Click event
    public void HandleClick()
    {
        _cameraDolly.RotateDolly(rotationAmount);
    }

}

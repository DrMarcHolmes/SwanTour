using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRStandardAssets.Utils;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class VRUISelection : MonoBehaviour
{
    private VRInteractiveItem _VRInteractiveItem;
    public Button button;

    private void Awake()
    {
        if (_VRInteractiveItem == null)
            _VRInteractiveItem = GetComponent<VRInteractiveItem>();

    }

    private void OnEnable()
    {
        if (_VRInteractiveItem == null)
            _VRInteractiveItem = GetComponent<VRInteractiveItem>();

        _VRInteractiveItem.OnOver += HandleOver;
        _VRInteractiveItem.OnOut += HandleOut;
        _VRInteractiveItem.OnClick += HandleClick;
    }

    //Handle the Over event
    public void HandleOver()
    {
        button.Select();
    }

    //Handle the Out event
    public void HandleOut()
    {
        EventSystem.current.SetSelectedGameObject(null);
    }

    //Handle the Click event
    public void HandleClick()
    {
        button.onClick.Invoke();
    }


    private void OnDisable()
    {
        if (_VRInteractiveItem == null)
            _VRInteractiveItem = GetComponent<VRInteractiveItem>();

        _VRInteractiveItem.OnOver -= HandleOver;
        _VRInteractiveItem.OnOut -= HandleOut;
        _VRInteractiveItem.OnClick -= HandleClick;
    }

}

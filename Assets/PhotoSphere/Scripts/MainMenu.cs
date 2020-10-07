using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    public CameraDolly desktopCameraDolly;
    public CameraDolly googleCameraDolly;
    public CameraDolly oculusCameraDolly;

    public GameObject vruiBackButton;
    public GameObject desktopControlsUI;

    private void Start()
    {
        desktopCameraDolly.ToggleSceneSelectionCamera();
        googleCameraDolly.ToggleSceneSelectionCamera();
        oculusCameraDolly.ToggleSceneSelectionCamera();

        vruiBackButton.SetActive(false);
        desktopControlsUI.SetActive(false);
    }
}

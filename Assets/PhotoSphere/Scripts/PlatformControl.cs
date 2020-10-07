using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
public class PlatformControl : MonoBehaviour
{
    public bool emulatePlatform;
    public BuildEmulation buildEmulation;
    public GameObject desktopController;
    public GameObject googleVRController;
    public GameObject oculusController;
    public CameraDolly activeDolly;
    public PhotosphereController photosphereController;
    private SceneData _sceneData;

    public enum BuildEmulation
    {
        Desktop,
        Android,
        Oculus,
    }

    private void Start()
    {
        if (photosphereController == null)
            photosphereController = GameObject.FindObjectOfType<PhotosphereController>();

        if (GameObject.FindObjectOfType<SceneData>() != null)
            _sceneData = GameObject.FindObjectOfType<SceneData>();

#if !UNITY_EDITOR
    emulatePlatform = false;
#endif

        if (!emulatePlatform)
        {
#if UNITY_STANDALONE || UNITY_WEBGL
            ConfigureDesktopControls();
            Debug.Log("Desktop Controls Loaded");

#elif UNITY_ANDROID || UNITY_IOS

            // check the current XR settings. Oculus and Google VR SDKs cannot be both active at the same time so will require 2 seperate APKs
            // find which one is active and set the controller to that.
            if (XRSettings.supportedDevices[0] == "cardboard" || XRSettings.supportedDevices[0] == "daydream")
            {
                ConfigureGoogleVRControls();
            }
            else if (XRSettings.supportedDevices[0] == "Oculus")
            {
                ConfigureOculusControls();
            }
            else
                Debug.LogError("No suitable Android device found, check XR SDK settings or device name");


#else
        Debug.Debug.LogError("No Controls Set Up For This Platform");
#endif
        }
        else
        {
            // if the desktop
            if (buildEmulation == BuildEmulation.Desktop)
                ConfigureDesktopControls();

            if (buildEmulation == BuildEmulation.Android)
                ConfigureGoogleVRControls();

            if (buildEmulation == BuildEmulation.Oculus)
                ConfigureOculusControls();
        }

        if (_sceneData != null)
            activeDolly.allowSceneSelection = _sceneData.allowSceneSelection;
    }
    public void ConfigureDesktopControls()
    {
        Debug.Log("Desktop Controls Loaded");

        activeDolly = desktopController.GetComponent<CameraDolly>();

        XRSettings.enabled = false;

        // destory google object
        Destroy(transform.GetChild(1).gameObject);

        desktopController.transform.SetParent(null);
        desktopController.SetActive(true);

        if (photosphereController != null)
            photosphereController.ConfigurePhotospheres(desktopController);
    }

    public void ConfigureGoogleVRControls()
    {
        Debug.Log("Google VR Controls Loaded");

        activeDolly = googleVRController.GetComponent<CameraDolly>();

        XRSettings.enabled = true;

        googleVRController.transform.SetParent(null);
        googleVRController.SetActive(true);

        if (photosphereController != null)
            photosphereController.ConfigurePhotospheres(googleVRController);
    }
    public void ConfigureOculusControls()
    {
        Debug.Log("Oculus VR Controls Loaded");

        activeDolly = oculusController.GetComponent<CameraDolly>();

        XRSettings.enabled = true;

        // turn on light
        transform.Find("Oculus/Directional Light").gameObject.SetActive(true);

        // destory google object
        Destroy(transform.GetChild(1).gameObject);

        oculusController.transform.SetParent(null);
        oculusController.SetActive(true);

        // turn on visualiser
        transform.Find("Oculus/SelectionVisualizer").gameObject.SetActive(true);

        if (photosphereController != null)
            photosphereController.ConfigurePhotospheres(oculusController);
    }
}

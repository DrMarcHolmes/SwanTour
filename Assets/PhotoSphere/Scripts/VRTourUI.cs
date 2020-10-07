using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class VRTourUI : MonoBehaviour
{
    public GameObject scenePanel;
    public GameObject loadingText;
    public PlatformControl platformControl;

    public void LoadScene(int scene)
    {
        StartCoroutine(TransitionScene(scene));
    }

    private IEnumerator TransitionScene(int scene)
    {
        scenePanel.SetActive(false);
        loadingText.SetActive(true);

        yield return new WaitForSeconds(1f);

        SceneManager.LoadScene(scene);
    }

    public void OnClickBackButton()
    {
        platformControl.activeDolly.ToggleSceneSelectionCamera();
    }
}

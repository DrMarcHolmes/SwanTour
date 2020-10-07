using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuitApp : MonoBehaviour
{
    private DesktopInput _desktopInput;

    private void Awake()
    {
        _desktopInput = transform.root.GetComponent<DesktopInput>();
    }

    public void OnClickQuit()
    {
        Application.Quit();
    }

    public void OnClickCancel()
    {
        _desktopInput.IsOnQuitMenu = false;
    }

}
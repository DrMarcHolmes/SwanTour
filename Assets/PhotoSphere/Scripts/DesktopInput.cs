using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DesktopInput : MonoBehaviour
{
    public CursorLockMode  _targetMode;
    private Transform _cameraTransform;
    private const string AXIS_MOUSE_X = "Mouse X";
    private const string AXIS_MOUSE_Y = "Mouse Y";
    private float mouseX = 0;
    private float mouseY = 0;
    private float mouseZ = 0;
    private bool _leaning;
    private bool _onQuitMenu;

    public Vector3 HeadPosition { get; private set; }
    public Quaternion HeadRotation { get; private set; }

    public GameObject pausePanel;
    public GameObject controlsPanel;
    public GameObject reticle;

    private CameraDolly _cameraDolly;

    public bool IsOnQuitMenu
    {
        get
        {
            return _onQuitMenu;
        }
        set
        {
            _onQuitMenu = value;
            pausePanel.SetActive(value);
            controlsPanel.SetActive(!value);
            reticle.SetActive(!value);

            if (!value)
            {
                _targetMode = CursorLockMode.Locked;
                SetCursorState();
            }
        }
    }

    private void Start()
    {
#if !UNITY_EDITOR
      _targetMode = CursorLockMode.Locked;
#endif
        SetCursorState();
        _cameraTransform = gameObject.transform.GetChild(1).transform;
        _cameraDolly = GetComponent<CameraDolly>();
    }

    private void SetCursorState()
    {
        Cursor.lockState = _targetMode;
        // Hide cursor when locking
        Cursor.visible = (CursorLockMode.Locked != _targetMode);
    }

    // Update is called once per frame
    private void Update()
    {
        

        if (Input.GetMouseButtonDown(0) && !IsOnQuitMenu)
        {
            reticle.SetActive(true);
            _targetMode = CursorLockMode.Locked;
        }
            
        if (Input.GetMouseButtonUp(0) && !IsOnQuitMenu)
        {
            reticle.SetActive(false);
            _targetMode = CursorLockMode.None;
        }
            

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = _targetMode = CursorLockMode.None;
            IsOnQuitMenu = true;
        }

        if (Input.GetKeyDown(KeyCode.Tab))
            _cameraDolly.ToggleSceneSelectionCamera();

        SetCursorState();

        if (Cursor.lockState == CursorLockMode.Locked)
            GetMouseInput();


        UpdateHeadPositionAndRotation();
        ApplyHeadOrientationToVRCameras();
    }

    private void GetMouseInput()
    {
        if (IsOnQuitMenu)
            return;

        if (!_leaning)
        {
            mouseX += Input.GetAxis(AXIS_MOUSE_X) * 5;
            if (mouseX <= -180)
            {
                mouseX += 360;
            }
            else if (mouseX > 180)
            {
                mouseX -= 360;
            }
            mouseY -= Input.GetAxis(AXIS_MOUSE_Y) * 2.4f;
            mouseY = Mathf.Clamp(mouseY, -85, 85);
        }

        if (CanChangeRoll())
        {
            _leaning = true;
            mouseZ += Input.GetAxis(AXIS_MOUSE_X) * 5;
            mouseZ = Mathf.Clamp(mouseZ, -85, 85);
        }
        else
            _leaning = false;

        if (!_leaning)
        {
            // People don't usually leave their heads tilted to one side for long.
            mouseZ = Mathf.Lerp(mouseZ, 0, Time.deltaTime / (Time.deltaTime + 0.1f));
        }
    }

    private bool CanChangeRoll()
    {
        return Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl);
    }

    private void UpdateHeadPositionAndRotation()
    {
        HeadRotation = Quaternion.Euler(mouseY, mouseX, mouseZ);
        HeadPosition = HeadRotation * Vector3.up;
    }
    private void ApplyHeadOrientationToVRCameras()
    {
        // unsure what this code was for
        //_cameraTransform.transform.localPosition = HeadPosition * _cameraTransform.transform.lossyScale.y;
        _cameraTransform.transform.localRotation = HeadRotation;
    }
}

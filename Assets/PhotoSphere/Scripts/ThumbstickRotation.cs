using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThumbstickRotation : MonoBehaviour
{

    public float rotationThreshold;
    private bool waitingToResetRightPrimary;
    private bool waitingToResetLeftPrimary;
    private bool waitingToResetRightSecondary;
    private bool waitingToResetLeftSecondary;

    public ArrowRotation rotaterRight;
    public ArrowRotation rotaterLight;


    // Update is called once per frame
    void Update()
    {
#if (!UNITY_ANDROID) // dont have thumbstick rotation for oculus go
        {
            PrimaryThumbstick();
            SecondaryThumbstick();
        }
#endif
    }

    private void PrimaryThumbstick()
    {
        // handle turn right
        if (Input.GetAxis("Oculus_CrossPlatform_PrimaryThumbstickHorizontal") > rotationThreshold && !waitingToResetRightPrimary)
        {
            waitingToResetRightPrimary = true;
            rotaterRight.HandleClick();
        }

        // reset right
        if (Input.GetAxis("Oculus_CrossPlatform_PrimaryThumbstickHorizontal") < 0.1f && waitingToResetRightPrimary)
        {
            waitingToResetRightPrimary = false;
        }

        // handle turn left
        if (Input.GetAxis("Oculus_CrossPlatform_PrimaryThumbstickHorizontal") < -rotationThreshold && !waitingToResetLeftPrimary)
        {
            waitingToResetLeftPrimary = true;
            rotaterLight.HandleClick();
        }

        // reset left
        if (Input.GetAxis("Oculus_CrossPlatform_PrimaryThumbstickHorizontal") > -0.1f && waitingToResetLeftPrimary)
        {
            waitingToResetLeftPrimary = false;
        }
    }

    private void SecondaryThumbstick()
    {
        // handle turn right
        if (Input.GetAxis("Oculus_CrossPlatform_SecondaryThumbstickHorizontal") > rotationThreshold && !waitingToResetRightPrimary)
        {
            waitingToResetRightPrimary = true;
            rotaterRight.HandleClick();
        }

        // reset right
        if (Input.GetAxis("Oculus_CrossPlatform_SecondaryThumbstickHorizontal") < 0.1f && waitingToResetRightPrimary)
        {
            waitingToResetRightPrimary = false;
        }

        // handle turn left
        if (Input.GetAxis("Oculus_CrossPlatform_SecondaryThumbstickHorizontal") < -rotationThreshold && !waitingToResetLeftSecondary)
        {
            waitingToResetLeftSecondary = true;
            rotaterLight.HandleClick();
        }

        // reset left
        if (Input.GetAxis("Oculus_CrossPlatform_SecondaryThumbstickHorizontal") > -0.1f && waitingToResetLeftSecondary)
        {
            waitingToResetLeftSecondary = false;
        }
    }
}

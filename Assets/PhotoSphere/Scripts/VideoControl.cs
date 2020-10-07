using System.Collections;
using System.Collections.Generic;
using UnityEngine.Video;
using UnityEngine;
using VRStandardAssets.Utils;

public class VideoControl : MonoBehaviour
{

    private VRInteractiveItem _VRInteractiveItem;
    private VideoPlayer _videoPlayer;

    private void OnEnable()
    {
        if (_VRInteractiveItem == null)
            _VRInteractiveItem = GetComponent<VRInteractiveItem>();


        _VRInteractiveItem.OnClick += HandleClick;
    }

    public void ReceiveVideoPlayer(VideoPlayer vp)
    {
        _videoPlayer = vp;
    }


    public void HandleClick()
    {
        if (_videoPlayer != null)
        {
            if (_videoPlayer.isPlaying)
            {
                _videoPlayer.Pause();
            }
            else
            {
                _videoPlayer.Play();
            }
        }
    }

    private void OnDisable()
    {
        if (_VRInteractiveItem == null)
            _VRInteractiveItem = GetComponent<VRInteractiveItem>();

        _VRInteractiveItem.OnClick -= HandleClick;
    }
}

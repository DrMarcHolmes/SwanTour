using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using VRStandardAssets.Utils;
using VRStandardAssets.Examples;

[ExecuteInEditMode]
[System.Serializable]
public class Photosphere : MonoBehaviour
{
    private PhotosphereController _photosphereController;
    private GameObject _photoGimbal;
    private GameObject _videoPlayer;

    private bool _enablePhotoGimbal;
    public bool PhotoGimbalEnabled
    {
        get { return _enablePhotoGimbal; }
        set
        {
            _enablePhotoGimbal = value;
            _photoGimbal.SetActive(_enablePhotoGimbal);
        }
    }

    private bool _hasBeenVisited;
    public bool HasBeenVisited
    {
        get { return _hasBeenVisited; }
        set
        {
            _hasBeenVisited = value;
            
            if (_hasBeenVisited && OnVisited != null)
                OnVisited();
        }
    }

    public delegate void VisitedAction();
    public static event VisitedAction OnVisited;

    private bool _isVideoSphere;
    public bool IsVideoSphere
    {
        get
        {
            return _isVideoSphere;
        }

        set
        {
            _isVideoSphere = value;
        }
    }

    private bool _playVideo;
    public bool PlayVideo
    {
        get { return _playVideo; }
        set
        {
            _playVideo = value;
            _videoPlayer.SetActive(_playVideo);
        }
    }

    private void Awake()
    {
        if (_videoPlayer == null)
            _videoPlayer = transform.Find("VideoPlayer").gameObject;

        if (_photoGimbal == null)
            _photoGimbal = transform.Find("PhotoGimbal").gameObject;
    }

    private void OnEnable()
    {
        if (!_photosphereController)
            _photosphereController = FindObjectOfType<PhotosphereController>();

        _photosphereController.AddPhotosphere(this);
    }

    private void Update()
    {
        if (Application.isPlaying == false)
        {
            if (_photoGimbal == null)
            {
                _photoGimbal = transform.Find("PhotoGimbal").gameObject;
            }

            if (_photoGimbal.transform.localPosition != Vector3.zero)
            {
                Debug.LogWarning("Photogimbal poistion cannot be moved, move parent Photosphere object instead");
                _photoGimbal.transform.localPosition = Vector3.zero;
            }
        }
    }

    public void SetPhotosphereImage(Texture image)
    {
        Shader shader = Shader.Find("Unlit/Monosphere");
        Material mat = new Material(shader);
        mat.SetTexture("_MainTex", image);

        transform.Find("PhotoGimbal").GetComponent<MeshRenderer>().sharedMaterial = mat;
    }

    public void SetPhotosphereVideo(VideoClip clip)
    {
        transform.Find("VideoPlayer").GetComponent<VideoPlayer>().clip = clip;
    }

    public void ConfigurePhotosphere(GameObject controller)
    {
        //disable outer transparent renderer
        MeshRenderer r = GetComponent<MeshRenderer>();

        if (r != null)
        {
            r.enabled = false;
        }


        // Check if this is a video sphere
        VideoPlayer videoPlayer = transform.Find("VideoPlayer").GetComponent<VideoPlayer>();

        _isVideoSphere = videoPlayer?.clip;

        if (_isVideoSphere)
        {
            transform.Find("Reticle Collider Parent").GetChild(0).GetComponent<VideoControl>().ReceiveVideoPlayer(videoPlayer);
        }

        TelportVrScript[] teleporters = transform.GetChild(1).GetComponentsInChildren<TelportVrScript>();

        for (int i = 0; i < teleporters.Length; i++)
        {
            teleporters[i].dolly = controller.transform;
        }
    }



}

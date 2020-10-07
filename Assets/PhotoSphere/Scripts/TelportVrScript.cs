using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using VRStandardAssets.Utils;

namespace VRStandardAssets.Examples
{
    // This script is a simple example of how an interactive item can
    // be used to change things on gameobjects by handling events.
    [ExecuteInEditMode]
    [System.Serializable]
    public class TelportVrScript : MonoBehaviour
    {
        public Transform dolly;

        [SerializeField]
        public string destinationSphereName; // serialized string which is used to look up the correct destination sphere

        private VRInteractiveItem _VRInteractiveItem;
        public bool AutoAlign = false;

        // [HideInInspector]
        [SerializeField]
        public bool alligned;

        [InspectorButton("RealignTelporterSphere")]
        public bool realign;


        public float DistanceMultiplyer = 5.0f;

        private Material _material;
        public Color normalColour;
        public Color highlightedColour;
        public Color previouslyVisitedColour;
        public float highlightSpeed;

        private PhotosphereController _photosphereController;
        private Transform _destinationTransform;
        private Photosphere _destinationPhotosphere;

        private Photosphere _photosphere;

        private void Awake()
        {
            _photosphere = transform.parent.parent.GetComponent<Photosphere>();

            if (_VRInteractiveItem == null)
                _VRInteractiveItem = GetComponent<VRInteractiveItem>();

            if (Application.isPlaying)
                _material = GetComponent<MeshRenderer>().material;

            if (_photosphereController == null)
                _photosphereController = transform.root.GetComponent<PhotosphereController>();
        }

        // used in editor through photosphere editor
        public void SetDestination(GameObject target)
        {
            destinationSphereName = target.name;

            if (_photosphereController == null)
                _photosphereController = transform.root.GetComponent<PhotosphereController>();

            _destinationTransform = _photosphereController.FindPhotosphereTransform(destinationSphereName);

            Debug.Log(_destinationTransform);
        }

        private void OnEnable()
        {
            if (_VRInteractiveItem == null)
                _VRInteractiveItem = GetComponent<VRInteractiveItem>();

            _VRInteractiveItem.OnOver += HandleOver;
            _VRInteractiveItem.OnOut += HandleOut;
            _VRInteractiveItem.OnClick += HandleClick;
        }


        private void Update()
        {
            if (destinationSphereName != "")
            {
                // old code
                // if a manual destination has been set in the inspector
                // if (destination != null)
                //  SetDestination(destination.gameObject);

                if (_destinationTransform == null)
                    _destinationTransform = _photosphereController.FindPhotosphereTransform(destinationSphereName);

                if (AutoAlign == true && !alligned)
                {
                    if (_destinationTransform != null)
                    {
                        Vector3 vector_sphere = _destinationTransform.position - transform.parent.parent.position;
                        this.transform.position = (transform.parent.parent.position + vector_sphere.normalized * DistanceMultiplyer);
                        transform.localPosition = new Vector3(transform.localPosition.x, 0, transform.localPosition.z);
                        alligned = true;
                    }
                    else
                        Debug.LogError("Problem with Photosphere teleporter");
                }
            }
        }

        private void RealignTelporterSphere()
        {
            Vector3 vector_sphere = _destinationTransform.position - transform.parent.parent.position;
            this.transform.position = (transform.parent.parent.position + vector_sphere.normalized * DistanceMultiplyer);
            transform.localPosition = new Vector3(transform.localPosition.x, 0, transform.localPosition.z);
            Debug.Log("Realigned");
        }


        //Handle the Over event
        public void HandleOver()
        {
            StopAllCoroutines();

            if (_destinationTransform != null)
            {
                _destinationPhotosphere = _destinationTransform.GetComponent<Photosphere>();
            }

            if (_destinationPhotosphere.HasBeenVisited)
                StartCoroutine(ChangeColour(previouslyVisitedColour));
            else
            {
                StartCoroutine(ChangeColour(highlightedColour));
            }
        }

        //Handle the Out event
        public void HandleOut()
        {
            // Debug.Log("Show out state");
            // m_Renderer.material = m_NormalMaterial;
            StopAllCoroutines();
            StartCoroutine(ChangeColour(normalColour));
        }

        //Handle the Click event
        public void HandleClick()
        {
            _destinationPhotosphere.PhotoGimbalEnabled = true;
            _destinationPhotosphere.HasBeenVisited = true;
            /// move camera to new position ; 
            if (_destinationTransform != null)
            {
                if (_photosphere.IsVideoSphere)
                    _photosphere.PlayVideo = false;

                dolly.transform.position = _destinationTransform.position;

                Photosphere nextPhotosphere = _destinationTransform.GetComponent<Photosphere>();

                if (nextPhotosphere.IsVideoSphere)
                    nextPhotosphere.PlayVideo = true;
            }

            _photosphere.PhotoGimbalEnabled = false;
        }

        private IEnumerator ChangeColour(Color target)
        {
            while (_material.color != target)
            {
                _material.color = Color.Lerp(_material.color, target, Time.deltaTime * highlightSpeed);
                yield return null;
            }
        }

        private void OnDisable()
        {
            if (_VRInteractiveItem == null)
                _VRInteractiveItem = GetComponent<VRInteractiveItem>();

            _VRInteractiveItem.OnOver -= HandleOver;
            _VRInteractiveItem.OnOut -= HandleOut;
            _VRInteractiveItem.OnClick -= HandleClick;
        }

        // draw lines between links
        void OnDrawGizmos()
        {
            if (_destinationTransform != null)
            {
                Vector3 direction = _destinationTransform.transform.position - transform.parent.parent.position;
                direction = direction.normalized * (direction.magnitude * 0.5f);

                Gizmos.color = Color.red;
                Gizmos.DrawRay(transform.parent.parent.position, direction);
            }
        }
    }
}
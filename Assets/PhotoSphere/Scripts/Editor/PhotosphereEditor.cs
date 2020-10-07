using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine.Video;
using System.Collections.Generic;
using VRStandardAssets.Examples;
using System;
using System.IO;
using UnityEditor.SceneManagement;

[InitializeOnLoadAttribute]
[System.Serializable]
public class PhotosphereEditor : EditorWindow
{
    private string photoSphereAssetsPath = "Assets/Example Photosphere Assets";
    private bool autoConnectPhotospheres = true;
    public Texture photoTexture = null;
    public Texture updatePhotoTexture = null;
    public VideoClip videoClip = null;
    public VideoClip updateVideoClip = null;
    private List<Photosphere> _selectedPhotospheres;
    private List<Photosphere> _autoConnectedPhotospheres; // for group creation
    private PluginImporter oculusAndroidPlugin;
    private PluginImporter oculusAndroidUniversalPlugin;

    static PhotosphereEditor()
    {
        EditorApplication.hierarchyChanged += OnHierarchyChanged;
    }

    static void OnHierarchyChanged()
    {
        if (!Application.isPlaying && Resources.FindObjectsOfTypeAll(typeof(PhotosphereEditor)).Length > 0)
        {
            PhotosphereController photosphereController = GameObject.FindObjectOfType<PhotosphereController>();

            if (photosphereController != null)
            {
                photosphereController.RemoveMissingPhotospheres();

                PhotosphereEditor photosphereEditor = (PhotosphereEditor)EditorWindow.GetWindow(typeof(PhotosphereEditor));
                photosphereEditor.CleanUpPhotospheres();
            }
            else
                Debug.LogWarning("No Photosphere Controller found, please select 'Create New Photosphere' if you want to ");
        }
    }
    // Add menu named "My Window" to the Window menu
    [MenuItem("Window/Photosphere Editor")]
    static void Init()
    {
        // Get existing open window or if none, make a new one:
        PhotosphereEditor window = (PhotosphereEditor)EditorWindow.GetWindow(typeof(PhotosphereEditor));
        window.Show();
    }

    Vector2 scrollPos;

    void OnGUI()
    {
        Rect r = position;

        EditorGUILayout.BeginVertical();
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.Width(position.width), GUILayout.Height(position.height));

        GUILayout.Label("Create Photospheres", EditorStyles.boldLabel);


        if (GUILayout.Button("Create New Photosphere Group", GUILayout.Width(400), GUILayout.Height(40)))
        {
            string path = EditorUtility.OpenFolderPanel("Select Photosphere Asset Folder", "", "");

            if (path != "")
            {
                path = "Assets" + path.Substring(Application.dataPath.Length);
                photoSphereAssetsPath = path;
                CreateNewPhotosphereGroup();
            }
        }

        autoConnectPhotospheres = EditorGUILayout.Toggle("Auto Connect", autoConnectPhotospheres);

        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

        if (GUILayout.Button("Create New Photosphere", GUILayout.Width(400), GUILayout.Height(40)))
        {
            if (photoTexture == null && videoClip == null)
                Debug.LogWarning("No Photosphere Image or Video selected, please select one before creating a Photosphere");
            else
                CreateNewPhotosphere(null, 0);
        }

        photoTexture = (Texture)EditorGUILayout.ObjectField("Photosphere Image", photoTexture, typeof(Texture), false);
        videoClip = (VideoClip)EditorGUILayout.ObjectField("Photosphere Video", videoClip, typeof(VideoClip), false);

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        EditorGUILayout.Space();

        GUILayout.Label("Edit Photosphere", EditorStyles.boldLabel);

        updatePhotoTexture = (Texture)EditorGUILayout.ObjectField("Photosphere Image", updatePhotoTexture, typeof(Texture), false);
        updateVideoClip = (VideoClip)EditorGUILayout.ObjectField("Photosphere Video", updateVideoClip, typeof(VideoClip), false);

        EditorGUILayout.Space();

        if (GUILayout.Button("Update Selected Photosphere", GUILayout.Width(250), GUILayout.Height(40)))
        {
            GetSelectedPhotospheres();
            UpdatePhotosphere();
        }

        if (GUILayout.Button("Delete Selected Photospheres", GUILayout.Width(200), GUILayout.Height(30)))
        {
            GetSelectedPhotospheres();
            DeletePhotospheres();
        }

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        EditorGUILayout.Space();

        GUILayout.Label("Photosphere Connections", EditorStyles.boldLabel);

        if (GUILayout.Button("Connect Selected Photospheres", GUILayout.Width(300), GUILayout.Height(40)))
        {
            GetSelectedPhotospheres();
            ConnectPhotospheres();
        }

        if (GUILayout.Button("Disconnect Selected Photospheres", GUILayout.Width(300), GUILayout.Height(40)))
        {
            GetSelectedPhotospheres();
            DisconnectPhotospheres();
        }

         if (GUILayout.Button("Find Oculus Version", GUILayout.Width(300), GUILayout.Height(40)))
        {
            Debug.Log(Directory.GetDirectories("Assets/Oculus/VR/Plugins")[0]);
       
        }

#if UNITY_ANDROID

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        EditorGUILayout.Space();

        GUILayout.Label("Build Settings", EditorStyles.boldLabel);

        if (GUILayout.Button("Configure Google VR Build Settings", GUILayout.Width(300), GUILayout.Height(30)))
        {
            ToggleOculusPlugins(false);

            string[] sdks = new string[2];
            sdks[0] = "daydream";
            sdks[1] = "cardboard";

            UnityEditorInternal.VR.VREditor.SetVREnabledDevicesOnTargetGroup(BuildTargetGroup.Android, sdks);

            if (EditorUtility.DisplayDialog("Change build settings?", "Are you sure you want to change Android build target to Google VR? This will restart Unity.", "Yes", "No"))
                RestartUnityEditor();
        }

        if (GUILayout.Button("Configure Oculus Go Build Settings", GUILayout.Width(300), GUILayout.Height(30)))
        {
            ToggleOculusPlugins(true);

            string[] sdks = new string[1];
            sdks[0] = "Oculus";

            UnityEditorInternal.VR.VREditor.SetVREnabledDevicesOnTargetGroup(BuildTargetGroup.Android, sdks);

            if (EditorUtility.DisplayDialog("Change build settings?", "Are you sure you want to change Android build target to Oculus? This will restart Unity.", "Yes", "No"))
                RestartUnityEditor();
        }

#endif

        EditorGUILayout.EndVertical();
        EditorGUILayout.EndScrollView();
    }

    private PhotosphereController FindPhotosphereController()
    {
        GameObject photosphereController = GameObject.Find("Photospheres");

        if (photosphereController != null)
        {
            return photosphereController.GetComponent<PhotosphereController>();
        }
        else
        {
            photosphereController = new GameObject();
            photosphereController.name = "Photospheres";
            return photosphereController.AddComponent<PhotosphereController>();
        }
    }

    #region Creating

    public void CreateNewPhotosphere(Transform parentTransform, int zOffset)
    {
        PhotosphereController photosphereController = FindPhotosphereController();

        var prefab = AssetDatabase.LoadAssetAtPath("Assets/Photosphere/Prefabs/Photosphere.prefab", typeof(UnityEngine.Object)) as GameObject;
        GameObject newPhotosphere = Instantiate(prefab);

        Photosphere photosphere = newPhotosphere.GetComponent<Photosphere>();
        photosphere.SetPhotosphereImage(photoTexture);
        photosphere.SetPhotosphereVideo(videoClip);

        // set parent
        if (parentTransform == null)
            newPhotosphere.transform.parent = photosphereController.transform;
        else
            newPhotosphere.transform.parent = parentTransform;

        // set position
        photosphere.transform.position = new Vector3(0, 0, zOffset);

        // set name
        if (videoClip == null)
            newPhotosphere.name = GetPhotosphereName(photosphereController, false);
        else
            newPhotosphere.name = GetPhotosphereName(photosphereController, true);

        // temp fix
        if (_autoConnectedPhotospheres != null)
            _autoConnectedPhotospheres.Add(photosphere);
    }

    private void CreateNewPhotosphereGroup()
    {
        PhotosphereController photosphereController = FindPhotosphereController();

        GameObject photosphereGroup = new GameObject();
        photosphereGroup.name = "Photosphere Group";

        photosphereGroup.transform.parent = photosphereController.transform;

        _autoConnectedPhotospheres = new List<Photosphere>();

        int zOffset = 0;

        string[] photoSphereAssets = AssetDatabase.FindAssets("t:texture, t:videoclip", new[] { photoSphereAssetsPath });

        // for each texture asset found,
        foreach (string asset in photoSphereAssets)
        {
            Type assetType = AssetDatabase.GetMainAssetTypeAtPath(AssetDatabase.GUIDToAssetPath(asset));

            if (assetType == typeof(Texture2D))
            {
                photoTexture = (Texture)AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(asset), typeof(Texture));
                photoTexture.name = Path.GetFileNameWithoutExtension(AssetDatabase.GUIDToAssetPath(asset));
            }
            else if (assetType == typeof(VideoClip))
            {
                videoClip = (VideoClip)AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(asset), typeof(VideoClip));
                videoClip.name = Path.GetFileNameWithoutExtension(AssetDatabase.GUIDToAssetPath(asset));
            }

            CreateNewPhotosphere(photosphereGroup.transform, zOffset);

            // space out photospheres 8 units apart
            zOffset = zOffset + 8;

            photoTexture = null;
            videoClip = null;
        }

        if (autoConnectPhotospheres)
        {
            for (int i = 0; i < _autoConnectedPhotospheres.Count; i++)
            {
                // dont need to connect last photosphere as it will already be connected
                if (i < _autoConnectedPhotospheres.Count - 1)
                {
                    _selectedPhotospheres.Clear();
                    _selectedPhotospheres.Add(_autoConnectedPhotospheres[i]);
                    _selectedPhotospheres.Add(_autoConnectedPhotospheres[i + 1]);
                    ConnectPhotospheres();
                }
            }
        }
    }

    #endregion

    #region Editing

    private string GetPhotosphereName(PhotosphereController photosphereController, bool isVideoSphere)
    {
        // set photosphere name, should be unique but check anyway
        string photosphereName;

        if (isVideoSphere)
            photosphereName = "Photosphere " + videoClip.name + " Video";
        else
            photosphereName = "Photosphere " + photoTexture.name;

        int duplicates = 0;
        for (int i = 0; i < photosphereController.photospheres.Count; i++)
        {
            if (photosphereController.photospheres[i].name == photosphereName)
            {
                duplicates++;

                if (isVideoSphere)
                    photosphereName = "Photosphere " + videoClip.name + " Video" + " " + duplicates;
                else
                    photosphereName = "Photosphere " + photoTexture.name + " " + duplicates;
            }
        }

        if (duplicates > 0)
            Debug.LogWarning("Duplicate Photosphere detected");

        return photosphereName;
    }

    private void UpdatePhotosphere()
    {
        if (_selectedPhotospheres.Count == 0)
        {
            Debug.Log("No Photosphere Selected");
            return;
        }

        if (_selectedPhotospheres.Count > 1)
        {
            Debug.LogWarning("More than one Photosphere selected. Only one Photosphere can be edited at a time");
            return;
        }

        _selectedPhotospheres[0].SetPhotosphereImage(updatePhotoTexture);
        _selectedPhotospheres[0].SetPhotosphereVideo(updateVideoClip);

        Debug.Log("Photosphere Updated");
    }

    private void DeletePhotospheres()
    {
        DisconnectPhotospheres();

        for (int i = 0; i < _selectedPhotospheres.Count; i++)
        {
            DestroyImmediate(_selectedPhotospheres[i].gameObject);
        }

        if (_selectedPhotospheres.Count > 1 || _selectedPhotospheres.Count == 0)
            Debug.Log(_selectedPhotospheres.Count + " Photospheres Deleted");
        else
            Debug.Log(_selectedPhotospheres.Count + " Photosphere Deleted");
    }

    // Find any broken teleporter connections to photospheres that no longer exist
    private void CleanUpPhotospheres()
    {
        PhotosphereController photosphereController = FindPhotosphereController();

        for (int i = 0; i < photosphereController.photospheres.Count; i++)
        {
            // get teleporters, only need to check ones that are enabled
            TelportVrScript[] teleporters = photosphereController.photospheres[i].gameObject.transform.GetChild(1).GetComponentsInChildren<TelportVrScript>(false);

            // check each teleporter to find missing connection
            for (int t = 0; t < teleporters.Length; t++)
            {
                if (photosphereController.FindPhotosphereTransform(teleporters[t].destinationSphereName) == null)
                {
                    DisconnectTeleporter(teleporters[t]);
                }
            }
        }
    }

    #endregion

    #region Connecting

    // connect the photospheres within _selectedPhotosphers. _selectedPhotospheres set from Selection.gameobjects or manually assigned within CreateNewPhotosphereGroup()
    private void ConnectPhotospheres()
    {
        if (_selectedPhotospheres.Count < 2)
        {
            Debug.LogWarning("Please select at least 2 Photospheres");
            return;
        }
        else if (_selectedPhotospheres.Count > 8)
        {
            Debug.LogWarning("A Maximum of 8 Photospheres can be connected, skipping excess Photospheres");
            _selectedPhotospheres.RemoveRange(8, _selectedPhotospheres.Count - 8);
        }

        Debug.Log("Connecting " + _selectedPhotospheres.Count + "  Photospheres");

        // for each selected photosphere    
        for (int i = 0; i < _selectedPhotospheres.Count; i++)
        {
            // get teleporters
            TelportVrScript[] teleporters = _selectedPhotospheres[i].gameObject.transform.GetChild(1).GetComponentsInChildren<TelportVrScript>(true);

            // for every other photopsphere it needs to connect to    
            for (int j = 0; j < _selectedPhotospheres.Count; j++)
            {
                // make sure its not connecting to itself
                if (j != i)
                {
                    // make sure the photosphere is all ready connected to the target
                    bool alreadyConnected = false;

                    for (int l = 0; l < teleporters.Length; l++)
                    {
                        if (teleporters[l].destinationSphereName == _selectedPhotospheres[j].gameObject.name)
                        {
                            teleporters[l].gameObject.SetActive(true);
                            alreadyConnected = true;
                        }
                    }

                    if (!alreadyConnected)
                    {
                        // try a teleporter, if its already occupied, try the next one
                        for (int t = 0; t < teleporters.Length; t++)
                        {
                            if (teleporters[t].destinationSphereName == "" && teleporters[t].destinationSphereName != _selectedPhotospheres[j].gameObject.name)
                            {
                                SerializedObject serializedObject = new UnityEditor.SerializedObject(teleporters[t]);
                                SerializedProperty serializedProperty = serializedObject.FindProperty("destinationSphereName");

                                serializedProperty.stringValue = _selectedPhotospheres[j].gameObject.name;

                                serializedProperty.serializedObject.ApplyModifiedProperties();
                                serializedProperty.serializedObject.Update();

                                teleporters[t].SetDestination(_selectedPhotospheres[j].gameObject);
                                teleporters[t].gameObject.SetActive(true);
                                break;
                            }


                            if (t == teleporters.Length - 1)
                                Debug.LogWarning("No free teleporters for Photosphere " + _selectedPhotospheres[i].gameObject.name);
                        }
                    }
                }
            }
        }
    }

    private void GetSelectedPhotospheres()
    {
        _selectedPhotospheres = new List<Photosphere>();

        for (int i = 0; i < Selection.gameObjects.Length; i++)
        {
            if (Selection.gameObjects[i].GetComponent<Photosphere>())
            {
                _selectedPhotospheres.Add(Selection.gameObjects[i].GetComponent<Photosphere>());
            }
        }
    }

    private void DisconnectPhotospheres()
    {
        Debug.Log("Disconnecting " + _selectedPhotospheres.Count + " Photospheres");

        PhotosphereController photosphereController = FindPhotosphereController();

        // for each selected photosphere    
        for (int i = 0; i < _selectedPhotospheres.Count; i++)
        {
            // get teleporters
            TelportVrScript[] teleporters = _selectedPhotospheres[i].gameObject.transform.GetChild(1).GetComponentsInChildren<TelportVrScript>(true);

            // disconnect each teleporter
            for (int t = 0; t < teleporters.Length; t++)
            {
                if (teleporters[t].destinationSphereName != "")
                {
                    // store this as will be wiped in DisconnectTeleporter
                    string sphereDestinationName = teleporters[t].destinationSphereName;

                    DisconnectTeleporter(teleporters[t]);

                    // check all other photospheres and disconnect them if necessary
                    for (int j = 0; j < photosphereController.photospheres.Count; j++)
                    {
                        if (photosphereController.photospheres[j].gameObject.name == sphereDestinationName)
                        {
                            TelportVrScript[] otherTeleporters = photosphereController.photospheres[j].gameObject.transform.GetChild(1).GetComponentsInChildren<TelportVrScript>(true);

                            // disconnect each other teleporter if it matches the first one we disconnected
                            for (int y = 0; y < otherTeleporters.Length; y++)
                            {
                                if (otherTeleporters[y].destinationSphereName == _selectedPhotospheres[i].gameObject.name)
                                {
                                    DisconnectTeleporter(otherTeleporters[y]);
                                }
                            }
                        }
                    }
                }
            }
        }
    }
    private void DisconnectTeleporter(TelportVrScript teleporter)
    {
        SerializedObject serializedObject = new UnityEditor.SerializedObject(teleporter);
        SerializedProperty serializedProperty = serializedObject.FindProperty("destinationSphereName");

        serializedProperty.stringValue = "";

        SerializedProperty serializedAllignment = serializedObject.FindProperty("alligned");

        serializedAllignment.boolValue = false;

        serializedProperty.serializedObject.ApplyModifiedProperties();
        serializedProperty.serializedObject.Update();

        teleporter.gameObject.SetActive(false);
    }

    #endregion

    #region Build Settings

    private void ToggleOculusPlugins(bool toggle)
    {
        oculusAndroidPlugin = AssetImporter.GetAtPath("Assets/Oculus/VR/Plugins/1.40.0/Android/OVRPlugin.aar") as PluginImporter;
        oculusAndroidUniversalPlugin = AssetImporter.GetAtPath("Assets/Oculus/VR/Plugins/1.40.0/AndroidUniversal/OVRPlugin.aar") as PluginImporter;

        oculusAndroidPlugin.SetCompatibleWithPlatform(BuildTarget.Android, toggle);
        oculusAndroidUniversalPlugin.SetCompatibleWithPlatform(BuildTarget.Android, toggle);
    }

    private static void RestartUnityEditor()
    {
        EditorApplication.OpenProject(GetCurrentProjectPath());
    }

    private static string GetCurrentProjectPath()
    {
        return Directory.GetParent(Application.dataPath).FullName;
    }

    #endregion

}
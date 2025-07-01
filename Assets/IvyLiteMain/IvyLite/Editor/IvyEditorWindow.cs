using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using UnityEditor;
using UnityEngine;

namespace IvyLite
{
    public class IvyEditorWindow : EditorWindow
    {
        #region 1) Main Variables
        /// <summary>
        /// Processes and exports 3D mesh data for Ivy branches and foliage instance properties.
        /// This function reads data files from specified directories, such as host mesh details, bounding boxes, and other parameters.
        /// Upon completion, it generates a 3D mesh representation for Ivy branches and outputs data files containing the Position, Scale, and Rotation attributes for foliage instances.
        /// </summary>
        /// <param name="lpermanentFolderPath">Directory path essential intermediary data files.</param>
        /// <param name="floatArray">Array of floats containing 120 distinct parameters.</param>
        /// <returns>Returns a float value, intended for error verification.</returns>
        /// 

        [DllImport("IvyProLib", CallingConvention = CallingConvention.Cdecl)]
        static extern float UnityOneMesh(string lpermanentFolderPath, float[] floatArray);

        // Lists representing different types of elements that will be instantiated on the plant.
        // For example, 'selectedLeaves' contains various types of leaves that will be scattered.
        private List<GameObject> selectedStems = new List<GameObject>();
        private List<GameObject> selectedLeaves = new List<GameObject>();
        private List<GameObject> selectedGranularItems = new List<GameObject>();
        private List<GameObject> selectedEndings = new List<GameObject>();

        // These are nested lists. For example, 'ivyLeaves' contains separate lists for each leaf type, 
        // and each of those inner lists contains instances of that specific leaf type.
        private List<List<GameObject>> ivyLeaves = new List<List<GameObject>>();
        private List<List<GameObject>> ivyGranularItems = new List<List<GameObject>>();
        private List<List<GameObject>> ivyEndings = new List<List<GameObject>>();
        private List<List<GameObject>> ivyStems = new List<List<GameObject>>();
        
        // A list contanining a set of distinct Parameters for each Granular Item in the list of selectedGranularItems
        private List<GranularParams> granularSettings = new List<GranularParams>();
        // List of Bools needed for FoldOuts Menus
        private bool[] foldouts;
        // Vector2 variables for the Scroll bars
        private Vector2 scrollPositionHost;


        // Vector 3 storing positions for the Initial point, target allows you choose a direction
        private Vector3 seed;
        private Vector3 target;

        /// Float array containing the parameters that dictate the growth of the plant.
        /// This array stores the values in the Settings Sliders and its passed to the Dll function UnityOneMesh 
        private float[] sliders;

        // Paths for the Permanent Unity folder and the root folder of the addon
        private string permanentFolderPath;
        private string mainFolderPath;

        private string currentPresetName;// String holding the Name of the currently selected Preset
        private Material ivyMaterial;// Material Applied to the ivyStem mesh
        private Material lineMaterial; // Material for the Line Renderer
        private float LineMultiplier;// Line Multiplier for Line Renderer

        private int ivyCounter = 0;// Ivy Counter
        private bool checkBaseMesh;//TRUE if Host Mesh has been found.
       

        // We need these bool Variables to start the placement of the Seed and ObjectiveType position using RayCast Picking on the Scene View
        private bool placeSeeds;
        private bool placeFirstPoint;
        private bool placeSecondPoint;
        // Bool Variable for Stem Line Renderer
        private bool lineRendererBranches;
        private float simplifyFC;
        private float dpiScaling = 1;//

        // ToolBar Variables
        private int toolbarOption = 2;// ToolBar selected Tab
        private string[] toolbarTextArray = { "Presets", "Settings", "Generator", "Optimization" };// Tabs Names
        
        
        // Instanciate MeshIO class
        MeshIO meshIO;        
       
        ///// UNCOMMENT TO ALLOW SETTING THE SEED AND TARGET WITH EMPTY GAMEOBJECTS
        // Transform references so you can attacht a game object to set the Seed and ObjectiveType positions        
        private Transform seedTransform;        
        private Transform targetTransform;

        //*************************************** Creates the Addon Window ********************
        private static IvyEditorWindow wInstance;
        [MenuItem("Tools/Ivy.Lite")]
        private static void ShowWindow()
        {
            wInstance = GetWindow<IvyEditorWindow>("Ivy.Lite");
            wInstance.minSize = new Vector2(640, 400);
        }
      
        #endregion       
        #region 2) Unity Functions
        private void OnEnable()
        {   
            // Handle for window instance
            wInstance = this;
            // Gets pixels per Point so the GUI layout works for different screen resolutions
            dpiScaling = EditorGUIUtility.pixelsPerPoint;
            // Array that stores the parameters coming from the setting sliders that we then pass to the DLL function
            sliders = new float[120];
            // Bool array that stores the state of foldable menu items
            foldouts = new bool[23];
            // Find the Main Folder Path
            mainFolderPath = Path.Combine(IvyProUtility.GetScriptPath("IvyEditorWindow", true) , "IvyLiteData/");
            //creates IvyMesh Folder If it doesn't exist.
            IvyProUtility.CreateIvyMeshFolder(mainFolderPath);
            // Assing MeshIO
            meshIO = new MeshIO(mainFolderPath);
            // Fills the default material for the Ivy Stems
            ivyMaterial = AssetDatabase.LoadAssetAtPath<Material>(Path.Combine(mainFolderPath , "Assets/Materials/IvyMat.mat"));

            // Load the Default Preset if it exists, If it doesn't exist the sliders take the values from SliderValues.bin
            string defaultPresetPath = Path.Combine(mainFolderPath, "Presets/Default.json");
            if (File.Exists(defaultPresetPath))
            {
                loadPresetWithPath(defaultPresetPath);
            }
            else
            {
                string filePath = Path.Combine(mainFolderPath, "SliderValues.bin");
                IvyProUtility.ReadFloatsFromBinaryFile(filePath, sliders);
            }
            // Create folders needed for data exchange 
            permanentFolderPath = Path.Combine(Application.persistentDataPath, "IvyLiteTemp");
            IvyProUtility.CreateTempFolders(permanentFolderPath);
            //Checks if there is 3D ( it will act as the Host Mesh) with name BaseMesh.bin inside the Temp 
            checkBaseMesh = File.Exists(Path.Combine(permanentFolderPath, "Temp/BaseMesh.bin"));           
            
            // Initialize the Ivy counter
            ivyCounter = IvyProUtility.InitializeIvyCount();
            meshIO.UpdateMeshCounter();
            // Initialize Line Renderer Multiplier
            LineMultiplier = 2.0f; // 2.0f because I'm passing the radius and this line width should be the diameter
            // Initialize the Line Renderer Material
            lineMaterial = AssetDatabase.LoadAssetAtPath<Material>(Path.Combine(mainFolderPath, "Assets/Materials/LineMat.mat"));
            // Initialize SimpliFy to 0.0f , 0 means no simplification
            simplifyFC = 0.0f;
        }

        private void OnDisable()
        {
            SavePresetAsDefault();
            //Unsubscribe Functions from sceneview if for some reason they were still subscribed (They Shouldn't be)
            SceneView.duringSceneGui -= PlaceOneSeed;
            SceneView.duringSceneGui -= PlaceBridge;

        }        

        private void OnGUI()
        {
            // Generate Tabs
            toolbarOption = GUILayout.Toolbar(toolbarOption, toolbarTextArray);
            switch (toolbarOption)
            {
                case 0:
                    DrawMeshesTab();
                    break;
                case 1:
                    DrawSettingsTab();
                    break;
                case 2:
                    DrawGenerateTab();
                    break;
                case 3:
                    DrawCleanUpTab();
                    break;
                default:
                    EditorGUILayout.LabelField("Something went wrong in the toolbar selection");
                    break;
            }

        }       
        #endregion
        #region 3) Custom Functions
        //This is the main method where the DLL function is called and the whole structure of the plant is created, foliage, end caps , Granular Items etc...
        private void CreateIvy()
        {
            if (sliders[61] > 0) // If there is at least 1 Selected Granular Item
            {
                sliders[55] = 1.0f; // it trigers the Dll bool to make Granular
                string dumbPath = Path.Combine(permanentFolderPath, "Export/FFdumblist.bin");
                ExportGranularSettingsAndBB(dumbPath); // Export bouding boxes to prevent Granular Elements intersect Host meshes                    
            }
            if (selectedStems.Count > 0)
            {
                string dumbPath = Path.Combine(permanentFolderPath, "Export/stemdumblist.bin");
                meshIO.ExportBoundingBoxes(selectedStems, dumbPath);

            }
            float ivyError = UnityOneMesh(permanentFolderPath, sliders);
            
            if (ivyError == 0.0f)
            {
                ivyCounter++;
                string IvyName = "IvyLite_" + currentPresetName + "_" + ivyCounter.ToString("D3");
                GameObject ivyParentObject = GameObject.Find(IvyName);
                // If the GameObject doesn't exist, create a new empty GameObject with the specified name
                if (ivyParentObject == null)
                {
                    ivyParentObject = new GameObject(IvyName);
                }                
                if (selectedStems.Count > 0)
                {
                    ivyStems.Add(ScatterStemQu(selectedStems, "stems.spc", "Stems", ivyParentObject));
                }
                else
                {
                    if (lineRendererBranches)
                    {
                        GameObject linesGameObject = new GameObject("BranchLines");
                        linesGameObject.transform.SetParent(ivyParentObject.transform);
                        string ivyFilePath = Path.Combine(permanentFolderPath, "Export/outivy.ivy");
                        IvyProUtility.CreateLines(ivyFilePath, linesGameObject, lineMaterial, LineMultiplier, sliders[29], simplifyFC);
                    }
                    else
                    {
                        string binFilePath = Path.Combine(permanentFolderPath, "Temp/IvyMesh.bin");
                        meshIO.ImportIvyMesh(binFilePath, ivyMaterial, ivyParentObject);
                    }

                }

                if (sliders[60] > 0 && sliders[37] > 0)
                {
                    ivyLeaves.Add(ScatterInstancesQu(selectedLeaves, "scatterPC.scp", "Foliage", ivyParentObject));
                }

                if (sliders[61] > 0.0f & sliders[55] > 0.0f)
                {

                    ivyGranularItems.Add(ScatterInstancesQu(selectedGranularItems, "FFscatterPC.scp", "GranularItems", ivyParentObject));
                }


                if (sliders[62] > 0.0f && (sliders[77] > 0.0f || sliders[78] > 0.0f))
                {
                    ivyEndings.Add(ScatterInstancesQu(selectedEndings, "EndscatterPC.scp", "EndCaps", ivyParentObject));
                }
                AssetDatabase.Refresh();
            }
            else
            {
                if (ivyError == -2.0f)
                {
                    Debug.Log("Ivy couldn't be generated, Maybe the Seed Location is inside the Host Mesh, error: " + ivyError);
                }
                else
                {
                    Debug.Log("Ivy couldn't be generated, error: " + ivyError);
                }

            }
        }
        // Selects a point in the GUI using picking with Ray casting ,
        // This function needs to be subscribed to the sceneView (SceneView.duringSceneGUI += PlaceOneSeed;) 
        private void PlaceOneSeed(SceneView sceneView)
        {
            if (!placeSeeds) return;
            Event currentEvent = Event.current;
            if (currentEvent.type == EventType.MouseDown && currentEvent.button == 0)
            {
                Ray ray = HandleUtility.GUIPointToWorldRay(currentEvent.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit))
                {
                    float surfaceOffset = sliders[17];
                    Vector3 seedPosition = hit.point + hit.normal * surfaceOffset;
                    sliders[80] = seedPosition.x;
                    sliders[81] = seedPosition.y;
                    sliders[82] = seedPosition.z;
                    Vector3 finalPosition = seedPosition + 0.05f * hit.normal;
                    sliders[83] = finalPosition.x;
                    sliders[84] = finalPosition.y;
                    sliders[85] = finalPosition.z;
                    seed = seedPosition;
                    target = finalPosition;
                    sliders[60] = selectedLeaves.Count;
                    sliders[61] = selectedGranularItems.Count;
                    sliders[55] = 0.0f;
                    sliders[62] = selectedEndings.Count;
                    CreateIvy();

                }
                placeSeeds = false;
                //When we are done we can Unsubscribe
                SceneView.duringSceneGui -= PlaceOneSeed;
                currentEvent.Use();
            }
        }
        // Selects 2 points in the GUI using picking with Ray casting.
        //This function needs to be subscribed to the sceneView(SceneView.duringSceneGUI += PlaceBridge;) 
        private void PlaceBridge(SceneView sceneView)
        {
            if (!placeFirstPoint && !placeSecondPoint) return;

            Event currentEvent = Event.current;
            if (currentEvent.type == EventType.MouseDown && currentEvent.button == 0)
            {
                float surfaceOffset = sliders[17];
                Ray ray = HandleUtility.GUIPointToWorldRay(currentEvent.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit))
                {
                    if (placeFirstPoint)
                    {

                        Vector3 seedPosition = hit.point + hit.normal * surfaceOffset;
                        placeFirstPoint = false;
                        placeSecondPoint = true;
                        sliders[80] = seedPosition.x;
                        sliders[81] = seedPosition.y;
                        sliders[82] = seedPosition.z;
                        seed = seedPosition;
                    }
                    else if (placeSecondPoint)
                    {
                        Vector3 finalPosition = hit.point + hit.normal * surfaceOffset;
                        placeSecondPoint = false;
                        sliders[83] = finalPosition.x;
                        sliders[84] = finalPosition.y;
                        sliders[85] = finalPosition.z;
                        target = finalPosition;
                        if (seedTransform != null && targetTransform != null)
                        {
                            seedTransform.position = seed;
                            targetTransform.position = target;                            
                        }
                        Repaint();
                        //Unsubscribe
                        SceneView.duringSceneGui -= PlaceBridge;
                        //Debug.Log("Seed placed fin at: " + finalPosition);                   

                    }
                }
                
                currentEvent.Use();
            }
        }
        //  gets the points stored by the By the Place Bridge and displays the generated plant in the Scene.
        private void makeIvyBridge()
        {
            sliders[80] = seed.x;
            sliders[81] = seed.y;
            sliders[82] = seed.z;
            sliders[83] = target.x;
            sliders[84] = target.y;
            sliders[85] = target.z;
            sliders[60] = selectedLeaves.Count;
            sliders[61] = selectedGranularItems.Count;
            sliders[62] = selectedEndings.Count;
            sliders[55] = 0.0f;
            CreateIvy();
        }
        //
        private Texture2D CreateTexture(int width, int height, Color color)
        {
            Texture2D texture = new Texture2D(width, height);
            for (int x = 0; x < texture.width; x++)
            {
                for (int y = 0; y < texture.height; y++)
                {
                    texture.SetPixel(x, y, color);
                }
            }
            texture.Apply();
            return texture;
        }
        // SCATTER : Reads a file containing, Position, Scale and Rotation in the form of Quaternions and apply the transformation to instances.
        private List<GameObject> ScatterInstancesQu(List<GameObject> ListOfGameObjects, string fileName, string parentName, GameObject parentObject1)
        {
            string dataFolder = Path.Combine(permanentFolderPath, "Export/");
            string thisPath = Path.Combine(dataFolder , fileName);
            List<GameObject> instances = new List<GameObject>();
            GameObject parentObject = new GameObject(parentName + " Group");
            parentObject.transform.SetParent(parentObject1.transform);
            if (File.Exists(thisPath))
            {
                using (BinaryReader reader = new BinaryReader(File.Open(thisPath, FileMode.Open)))
                {
                    int jump = 12;
                    reader.BaseStream.Seek(jump, SeekOrigin.Begin);
                    uint numMeshes = reader.ReadUInt32();
                    uint totalInstances = reader.ReadUInt32();

                    for (int i = 0; i < totalInstances; i++)
                    {
                        uint meshIndex = reader.ReadUInt32();
                        float Qx = reader.ReadSingle();
                        float Qy = reader.ReadSingle();
                        float Qz = reader.ReadSingle();
                        float Qw = reader.ReadSingle();
                        float scale = reader.ReadSingle();
                        float x = reader.ReadSingle();
                        float y = reader.ReadSingle();
                        float z = reader.ReadSingle();

                        int indexint = (int)meshIndex;

                        if (meshIndex < numMeshes)
                        {
                            GameObject meshPrefab = ListOfGameObjects[indexint];
                            if (meshPrefab == null)
                            {
                                Debug.LogError("Invalid mesh prefab at index " + indexint);
                                continue;
                            }

                            Quaternion finalRotation = new Quaternion(Qx, Qy, Qz, Qw);

                            GameObject instance = Instantiate(meshPrefab, new Vector3(x, y, z), finalRotation);

                            instance.transform.localScale *= scale;
                            instance.transform.SetParent(parentObject.transform);
                            instances.Add(instance);

                        }
                        else
                        {
                            Debug.LogError("Mesh index " + meshIndex + " is out of bounds.");
                        }
                    }
                }
            }
            return instances;
        }
        // STEM SCATTER : Reads a file containing, Position, Scale and Rotation in the form of Quaternions
        // and apply the transformation to Stem instances.
        private List<GameObject> ScatterStemQu(List<GameObject> ListOfGameObjects, string fileName, string parentName, GameObject parentObject1)
        {
            string dataFolder = Path.Combine(permanentFolderPath, "Export/");
            string thisPath = Path.Combine(dataFolder, fileName);
            List<GameObject> instances = new List<GameObject>();
            GameObject parentObject = new GameObject(parentName + " Group");
            parentObject.transform.SetParent(parentObject1.transform);
            if (File.Exists(thisPath))
            {
                using (BinaryReader reader = new BinaryReader(File.Open(thisPath, FileMode.Open)))
                {
                    int jump = 12;
                    reader.BaseStream.Seek(jump, SeekOrigin.Begin);
                    uint numMeshes = reader.ReadUInt32();
                    uint totalInstances = reader.ReadUInt32();

                    for (int i = 0; i < totalInstances; i++)
                    {
                        uint meshIndex = reader.ReadUInt32();
                        float Qx = reader.ReadSingle();
                        float Qy = reader.ReadSingle();
                        float Qz = reader.ReadSingle();
                        float Qw = reader.ReadSingle();
                        float scaleX = reader.ReadSingle();
                        float scaleY = reader.ReadSingle();
                        float scaleZ = reader.ReadSingle();
                        float x = reader.ReadSingle();
                        float y = reader.ReadSingle();
                        float z = reader.ReadSingle();

                        int indexint = (int)meshIndex;

                        if (meshIndex < numMeshes)
                        {
                            GameObject meshPrefab = ListOfGameObjects[indexint];
                            if (meshPrefab == null)
                            {
                                Debug.LogError("Invalid mesh prefab at index " + indexint);
                                continue;
                            }

                            Quaternion finalRotation = new Quaternion(Qx, Qy, Qz, Qw);

                            GameObject instance = Instantiate(meshPrefab, new Vector3(x, y, z), finalRotation);

                            instance.transform.localScale = new Vector3(scaleX, scaleY, scaleZ);
                            instance.transform.SetParent(parentObject.transform);
                            instances.Add(instance);

                        }
                        else
                        {
                            Debug.LogError("Mesh index " + meshIndex + " is out of bounds.");
                        }
                    }
                }
            }
            return instances;
        }
        

        // Opens a File dialog and Load the selected file Json file Preset.
        private void loadPresetWithDialog()
        {
            IvyPreset preset = new IvyPreset();
            // Create a new instance of the PresetManager class, passing in the IvyPreset instance
            PresetManager presetManager = new PresetManager(preset, Path.Combine(mainFolderPath, "Presets"));
            // load the preset
            bool Loadit = presetManager.LoadPreset();
            if (preset != null && Loadit)
            {
                currentPresetName = preset.PresetName;
                selectedStems.Clear();
                selectedLeaves.Clear();
                selectedEndings.Clear();
                selectedGranularItems.Clear();
                granularSettings.Clear();

                selectedStems = preset._StemPaths.Select(path => AssetDatabase.LoadAssetAtPath<GameObject>(path)).ToList();
                selectedLeaves = preset._LeavesPaths.Select(path => AssetDatabase.LoadAssetAtPath<GameObject>(path)).ToList();
                selectedEndings = preset._EndingsPaths.Select(path => AssetDatabase.LoadAssetAtPath<GameObject>(path)).ToList();
                selectedGranularItems = preset._FruitsPaths.Select(path => AssetDatabase.LoadAssetAtPath<GameObject>(path)).ToList();
                sliders = preset.Prsliders;
                granularSettings = preset._fruitSettins;
                Material presetmat = AssetDatabase.LoadAssetAtPath<Material>(preset._IvyMaterialPath);
                if (presetmat != null)
                {
                    ivyMaterial = presetmat;
                }
            }
            else
            {
                currentPresetName = "No Preset";
                selectedStems.Clear();
                selectedLeaves.Clear();
                selectedEndings.Clear();
                selectedGranularItems.Clear();
                granularSettings.Clear();
                string filePath = Path.Combine(mainFolderPath, "SliderValues.bin");
                IvyProUtility.ReadFloatsFromBinaryFile(filePath, sliders);
            }
        }
        /// <summary>
        /// Open a file Dialog and saves a preset a Json file
        /// </summary>
        private void SavePresetWithDialog()
        {
            IvyPreset preset = new IvyPreset();
            {
                preset._StemPaths = selectedStems.Select(stem => IvyProUtility.GetAssetPathOrPrefabPath(stem)).ToList();
                preset._LeavesPaths = selectedLeaves.Select(leaf => IvyProUtility.GetAssetPathOrPrefabPath(leaf)).ToList();
                preset._EndingsPaths = selectedEndings.Select(ending => IvyProUtility.GetAssetPathOrPrefabPath(ending)).ToList();
                preset._FruitsPaths = selectedGranularItems.Select(fruit => IvyProUtility.GetAssetPathOrPrefabPath(fruit)).ToList();
                preset.Prsliders = sliders;
                preset._fruitSettins = granularSettings.ToList();
                preset._IvyMaterialPath = AssetDatabase.GetAssetPath(ivyMaterial);

            };
            // Create a new instance of the PresetManager class, passing in the IvyPreset instance
            PresetManager presetManager = new PresetManager(preset, Path.Combine(mainFolderPath, "Presets"));
            // Save the preset
            presetManager.SavePreset();
            currentPresetName = preset.PresetName;
        }
        private void SavePresetAsDefault()
        {
            IvyPreset preset = new IvyPreset();
            {
                preset._StemPaths = selectedStems.Select(stem => IvyProUtility.GetAssetPathOrPrefabPath(stem)).ToList();
                preset._LeavesPaths = selectedLeaves.Select(leaf => IvyProUtility.GetAssetPathOrPrefabPath(leaf)).ToList();
                preset._EndingsPaths = selectedEndings.Select(ending => IvyProUtility.GetAssetPathOrPrefabPath(ending)).ToList();
                preset._FruitsPaths = selectedGranularItems.Select(fruit => IvyProUtility.GetAssetPathOrPrefabPath(fruit)).ToList();
                preset.Prsliders = sliders;
                preset._fruitSettins = granularSettings.ToList();
                preset._IvyMaterialPath = AssetDatabase.GetAssetPath(ivyMaterial);

            };
            // Create a new instance of the PresetManager class, passing in the IvyPreset instance
            PresetManager presetManager = new PresetManager(preset, Path.Combine(mainFolderPath, "Presets"));
            // Save the preset
            string defaultPresetPath = Path.Combine(mainFolderPath, "Presets/Default.json");

            if (defaultPresetPath.Length != 0)
            {
                preset.PresetName = System.IO.Path.GetFileNameWithoutExtension(defaultPresetPath);
                // Convert the preset object to JSON
                string jsonString = JsonUtility.ToJson(preset, true);

                // Write the JSON string to a file
                File.WriteAllText(defaultPresetPath, jsonString);

                // Refresh the AssetDatabase to include the newly created file
                AssetDatabase.Refresh();
            }
            currentPresetName = preset.PresetName;
        }
        // Loads a Preset given a Path to a preset Json file
        private void loadPresetWithPath(string presetPath)
        {
            IvyPreset preset = new IvyPreset();
            // Create a new instance of the PresetManager class, passing in the IvyPreset instance
            PresetManager presetManager = new PresetManager(preset, Path.Combine(mainFolderPath, "Presets"));
            // load the preset
            bool Loadit = presetManager.LoadPresetWithPath(presetPath);
            if (preset != null && Loadit)
            {
                currentPresetName = preset.PresetName;
                selectedStems.Clear();
                selectedLeaves.Clear();
                selectedEndings.Clear();
                selectedGranularItems.Clear();
                granularSettings.Clear();

                selectedStems = preset._StemPaths.Select(path => AssetDatabase.LoadAssetAtPath<GameObject>(path)).ToList();
                selectedLeaves = preset._LeavesPaths.Select(path => AssetDatabase.LoadAssetAtPath<GameObject>(path)).ToList();
                selectedEndings = preset._EndingsPaths.Select(path => AssetDatabase.LoadAssetAtPath<GameObject>(path)).ToList();
                selectedGranularItems = preset._FruitsPaths.Select(path => AssetDatabase.LoadAssetAtPath<GameObject>(path)).ToList();
                sliders = preset.Prsliders;
                granularSettings = preset._fruitSettins;
                Material presetmat = AssetDatabase.LoadAssetAtPath<Material>(preset._IvyMaterialPath);
                if(presetmat != null)
                {
                    ivyMaterial = presetmat;
                }

            }
            else
            {
                currentPresetName = "No Preset";
                selectedStems.Clear();
                selectedLeaves.Clear();
                selectedEndings.Clear();
                selectedGranularItems.Clear();
                granularSettings.Clear();
                string filePath = Path.Combine(mainFolderPath, "SliderValues.bin");
                IvyProUtility.ReadFloatsFromBinaryFile(filePath, sliders);
            }
        }
        // Save a file containing the Settings for Granular Items
        private void ExportGranularSettingsAndBB(string outputPath)
        {
            using (BinaryWriter writer = new BinaryWriter(File.Open(outputPath, FileMode.Create)))
            {
                if (granularSettings.Count() != selectedGranularItems.Count())
                {
                    Debug.Log("Error Exporting Granular Item BoundingBox Settings");
                    return;
                }
                // Write the number of meshes
                writer.Write(selectedGranularItems.Count);
                int i = 0;
                foreach (GameObject meshObj in selectedGranularItems)
                {
                    // Write Settings for each GameObject
                    writer.Write(granularSettings[i].count);
                    writer.Write(granularSettings[i].angleSlope);
                    writer.Write(granularSettings[i].angleVariation);
                    writer.Write(granularSettings[i].scale * sliders[73]);// Multiply by Master Scale
                    writer.Write(granularSettings[i].scaleVariation);
                    writer.Write(granularSettings[i].easing);
                    writer.Write(granularSettings[i].minSize);
                    writer.Write(granularSettings[i].minAngle);
                    writer.Write(granularSettings[i].maxAngle);
                    writer.Write(granularSettings[i].proximal);
                    writer.Write(granularSettings[i].distal);
                    writer.Write(granularSettings[i].minDistance);
                    writer.Write(granularSettings[i].onMain);
                    writer.Write(granularSettings[i].onBranch);
                    writer.Write(granularSettings[i].onCatenary);
                    writer.Write(granularSettings[i].onAir);
                    writer.Write(granularSettings[i].hostNormal);
                    writer.Write(granularSettings[i].maxDistance);
                    writer.Write(granularSettings[i].normalMin);
                    writer.Write(granularSettings[i].normalMax);
                    writer.Write(granularSettings[i].normalAngleVariation);
                    writer.Write(granularSettings[i].surfaceOffset);
                    // Get the bounding box

                    MeshRenderer[] meshes= meshObj.GetComponentsInChildren<MeshRenderer>();
                    Bounds bounds =  meshes[0].bounds;
                    for( int j= 1; j < meshes.Length; j++ )
                    {
                        bounds.Encapsulate(meshes[j].bounds);
                    }
                   

                    // Write the min and max coordinates and the center point
                    writer.Write(bounds.min.x);
                    writer.Write(bounds.min.y);
                    writer.Write(bounds.min.z);
                    writer.Write(bounds.max.x);
                    writer.Write(bounds.max.y);
                    writer.Write(bounds.max.z);
                    writer.Write(bounds.center.x);
                    writer.Write(bounds.center.y);
                    writer.Write(bounds.center.z);
                    i++;
                }
            }
        }
        // Creates a Prefab in the mesh combiner section, saving the new combined meshes in a dedicated folder.  
        private void CreatePrefab(int idx)
        {
            string MeshesFolderPath = Path.Combine(mainFolderPath , "IvyMeshes/");
            string path = EditorUtility.SaveFilePanel("Save prefab as...", MeshesFolderPath, "IvyPrefab" + ".prefab", "prefab");
            path = FileUtil.GetProjectRelativePath(path);

            Object prefab = PrefabUtility.SaveAsPrefabAsset(meshIO.selectedIvys[idx], path);
            if (prefab == null)
            {
                Debug.LogError("Failed to create prefab at " + path);
            }

            if (path.Length != 0)
            {
                // Make sure the file path is relative to the Assets folder
                if (path.StartsWith(Application.dataPath))
                {
                    path = "Assets" + path.Substring(Application.dataPath.Length);
                }

                string directoryPath = Path.GetDirectoryName(path);

                string prefabName = Path.GetFileNameWithoutExtension(path);

                // Create a directory for the meshes
                string meshDirectoryPath = Path.Combine(directoryPath, prefabName + "_Meshes");

                if (!Directory.Exists(meshDirectoryPath))
                {
                    Directory.CreateDirectory(meshDirectoryPath);
                }

                MeshFilter[] meshFilters = meshIO.selectedIvys[idx].GetComponentsInChildren<MeshFilter>();

                // Create a set of unique meshes
                HashSet<Mesh> uniqueMeshes = new HashSet<Mesh>();
                foreach (MeshFilter meshFilter in meshFilters)
                {
                    if (meshFilter.sharedMesh != null)
                    {
                        uniqueMeshes.Add(meshFilter.sharedMesh);
                    }
                }

                // Create a dictionary to map the original meshes to the new meshes
                Dictionary<Mesh, Mesh> meshMap = new Dictionary<Mesh, Mesh>();

                // Create a copy for each unique mesh
                foreach (Mesh originalMesh in uniqueMeshes)
                {

                    Mesh meshCopy = Mesh.Instantiate(originalMesh) as Mesh;


                    meshCopy.name = "Copy_" + originalMesh.name;

                    string meshPath = Path.Combine(meshDirectoryPath, meshCopy.name + ".asset");


                    AssetDatabase.CreateAsset(meshCopy, meshPath);

                    // Add the original mesh and the copy to the dictionary
                    meshMap.Add(originalMesh, meshCopy);
                }

                // Update the MeshFilters to point to the new meshes
                foreach (MeshFilter meshFilter in meshFilters)
                {
                    if (meshFilter.sharedMesh != null)
                    {
                        meshFilter.sharedMesh = meshMap[meshFilter.sharedMesh];
                    }
                }

                // Create the new prefab
                Object Lprefab = PrefabUtility.SaveAsPrefabAsset(meshIO.selectedIvys[idx], path);
                if (Lprefab == null)
                {
                    Debug.LogError("Failed to create prefab at " + path);
                }
            }
        }
        #endregion
        #region 4) GUI
        //**** PRESET TAB ************************************
        #region 4.1) PRESET TAB 
        private void DrawMeshesTab()
        {
            float LcolumnWidth = Screen.width / (2 * dpiScaling);
            GUILayout.Label("PRESETS. Upgrade to IvyPro to Create and Save Presets ");
            ////
            if (GUILayout.Button(new GUIContent("Convert Pro Presets to Lite ", "Corrects the paths in IvyPro presets to ensure they point to the correct locations.")))
            {
                IvyProUtility.FixPathsProToLite(mainFolderPath);
            }
            if (GUILayout.Button(new GUIContent("Fix Presets Paths", "Use it when the location of the Prefabs has changed. Corrects the paths in presets to ensure they point to the correct locations.")))
            {
                IvyProUtility.FixPaths(mainFolderPath);
            }
        }
        #endregion
        //**** SETTINGS TAB **********************************
        #region 4.2) SETTINGS TAB
        private void DrawSettingsTab()
        {
            float LcolumnWidth = Screen.width / (2 * dpiScaling);
            GUILayout.Label("SETTINGS - Upgrade to IvyPro to Access all the settings ");
            ////
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.BeginVertical(GUI.skin.box, GUILayout.Width(LcolumnWidth));
            /////
            foldouts[0] = EditorGUILayout.BeginFoldoutHeaderGroup(foldouts[0], "General Settings");
            if (foldouts[0])
            {
                sliders[73] = EditorGUILayout.Slider(new GUIContent("Master Scale", " Affects Ivy's overall Scale"), sliders[73], 0.01f, 10.0f);

                sliders[64] = EditorGUILayout.IntSlider(new GUIContent("Bounds Kill",
                    "Set to 0 (Off) for unlimited growth, 1 (XYZ On) to halt growth at bounding box boundaries, or 2 (Y-Minimum) to stop growth below the minimum Y boundary.\""),
                    Mathf.RoundToInt(sliders[64]), 0, 2);

            }
            EditorGUILayout.EndFoldoutHeaderGroup();
            foldouts[1] = EditorGUILayout.BeginFoldoutHeaderGroup(foldouts[1], "Ivy Mesh Attributes");
            if (foldouts[1])
            {            

               
                sliders[2] = EditorGUILayout.Slider("Segment Length", sliders[2], 1.0f, 200.0f);
                sliders[3] = EditorGUILayout.Slider("Segment Length Variation", sliders[3], 0.0f, 200.0f);
               
                sliders[5] = EditorGUILayout.Slider("Radius", sliders[5], 0.0025f, 0.05f);
                if (sliders[5] < sliders[112])
                {
                    sliders[5] = sliders[112];
                }
                sliders[112] = EditorGUILayout.Slider("Minimum Radius ", sliders[112], 0.001f, 0.02f);
                sliders[29] = EditorGUILayout.Slider("Radius FallOff ", sliders[29], 0.9f, 1.000f);
                sliders[30] = EditorGUILayout.Slider("Twist", sliders[30], -45.0f, 45.0f);

                EditorGUILayout.LabelField("Activate Line Renderer for Branches", GUILayout.Width(LcolumnWidth));
                lineRendererBranches = EditorGUILayout.Toggle(lineRendererBranches, GUILayout.Width(LcolumnWidth * 0.10f));
                if (lineRendererBranches)
                {
                    LineMultiplier = EditorGUILayout.Slider("Line Multiplier",LineMultiplier, 0.01f, 5.0f);
                    simplifyFC = EditorGUILayout.Slider("Simplify", simplifyFC, 0.0f, 0.1f);
                    
                }
            }
            EditorGUILayout.EndFoldoutHeaderGroup();
            foldouts[18] = EditorGUILayout.BeginFoldoutHeaderGroup(foldouts[18], "Main Stem Settings");
            if (foldouts[18])
            {

                sliders[1] = EditorGUILayout.IntSlider("Main Stem Length", Mathf.RoundToInt(sliders[1]), 1, 1000);
                if (sliders[1] < sliders[9])
                {
                    sliders[1] = sliders[9] + 1.0f;
                }
                sliders[9] = EditorGUILayout.IntSlider("Minimum Length", Mathf.RoundToInt(sliders[9]), 2, 100);
                sliders[79] = EditorGUILayout.IntSlider("On Main", Mathf.RoundToInt(sliders[79]), 0, 100);
                sliders[68] = EditorGUILayout.Slider("Proximal Branching ", sliders[68], 0.0f, 0.9f);
                sliders[67] = EditorGUILayout.Slider("Distal Branching ", sliders[67], 0.1f, 1.0f);
                if (sliders[67] < sliders[68])
                {
                    sliders[67] = sliders[68] + 0.1f;
                }

                foldouts[2] = EditorGUILayout.Foldout(foldouts[2], "Main Stem 3D Forces");
                if (foldouts[2])
                {

                    GUILayout.Label("Upgrade to IvyPro to Access all the Settings ");
                }
                foldouts[3] = EditorGUILayout.Foldout(foldouts[3], "Main Stem Surface Forces");
                if (foldouts[3])
                {
                    GUILayout.Label("Upgrade to IvyPro to Access all the Settings ");
                }
            }

            EditorGUILayout.EndFoldoutHeaderGroup();

            foldouts[17] = EditorGUILayout.BeginFoldoutHeaderGroup(foldouts[17], "Branches Settings");
            if (foldouts[17])
            {
                sliders[11] = EditorGUILayout.IntSlider("Branches Count", Mathf.RoundToInt(sliders[11]), 0, 5000);
                sliders[12] = EditorGUILayout.IntSlider("Branches Length ", Mathf.RoundToInt(sliders[12]), 1, 500);
                sliders[46] = EditorGUILayout.IntSlider("Minimum Length", Mathf.RoundToInt(sliders[46]), 2, 50);
                sliders[63] = EditorGUILayout.IntSlider("Branching Angle ", Mathf.RoundToInt(sliders[63]), 1, 90);
                sliders[0] = EditorGUILayout.Slider("Proximal Branching ", sliders[0], 0.0f, 0.9f);
                sliders[89] = EditorGUILayout.Slider("Distal Branching ", sliders[89], 0.1f, 1.0f);
                if (sliders[89] < sliders[0])
                {
                    sliders[89] = sliders[0] + 0.1f;
                }
                sliders[104] = EditorGUILayout.IntSlider("Branches on Main", Mathf.RoundToInt(sliders[104]), 0, 100);
                sliders[105] = EditorGUILayout.IntSlider("Branches on Branch", Mathf.RoundToInt(sliders[105]), 0, 100);
                sliders[107] = EditorGUILayout.IntSlider("Branches on Catenary", Mathf.RoundToInt(sliders[107]), 0, 100);
                
                foldouts[19] = EditorGUILayout.Foldout(foldouts[19], "Branches 3D Forces");
                if (foldouts[19])
                {
                    GUILayout.Label("Upgrade to IvyPro to Access all the Settings ");
                }
                foldouts[20] = EditorGUILayout.Foldout(foldouts[20], "Branches Surface Forces");
                if (foldouts[20])
                {
                    GUILayout.Label("Upgrade to IvyPro to Access all the Settings ");
                }
            }
            EditorGUILayout.EndFoldoutHeaderGroup();

            foldouts[4] = EditorGUILayout.BeginFoldoutHeaderGroup(foldouts[4], "Surface Control");
            if (foldouts[4])
            {
                GUILayout.Label("Upgrade to IvyPro to Access all the Settings ");
            }
            EditorGUILayout.EndFoldoutHeaderGroup();

            EditorGUILayout.EndVertical();
            EditorGUILayout.BeginVertical(GUI.skin.box, GUILayout.Width(LcolumnWidth));

            foldouts[5] = EditorGUILayout.BeginFoldoutHeaderGroup(foldouts[5], "Catenary Control");
            if (foldouts[5])
            {
                GUILayout.Label("Upgrade to IvyPro to Access all the Settings ");
            }
            EditorGUILayout.EndFoldoutHeaderGroup();

            foldouts[6] = EditorGUILayout.BeginFoldoutHeaderGroup(foldouts[6], new GUIContent("Rambling Branches",
                " Rambling Branches are branches that grow independently from the constraints of the base mesh surface."));
            if (foldouts[6])
            {
                GUILayout.Label("Upgrade to IvyPro to Access all the Settings ");
            }
            EditorGUILayout.EndFoldoutHeaderGroup();

            foldouts[7] = EditorGUILayout.BeginFoldoutHeaderGroup(foldouts[7], "Foliage Settings");
            if (foldouts[7])
            {
                GUILayout.Label("Upgrade to IvyPro to Access all the Settings ");

            }
            EditorGUILayout.EndFoldoutHeaderGroup();
            foldouts[8] = EditorGUILayout.BeginFoldoutHeaderGroup(foldouts[8], "End Cap Settings");
            if (foldouts[8])
            {
                GUILayout.Label("Upgrade to IvyPro to Access all the Settings ");
            }
            EditorGUILayout.EndFoldoutHeaderGroup();
            foldouts[9] = EditorGUILayout.BeginFoldoutHeaderGroup(foldouts[9], "Scatter Settings ");
            if (foldouts[9])
            {
                EditorGUILayout.LabelField(" Scatter Meshes Count", sliders[55].ToString());

            }
            EditorGUILayout.EndFoldoutHeaderGroup();
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndHorizontal();
           
        }
        #endregion
        //**** GERENATE TAB **********************************
        #region 4.3) GENERATE
        private void DrawGenerateTab()
        {
            float LcolumnWidth = Screen.width / (2 * dpiScaling);
            GUILayout.Label("GENERATOR");

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.BeginVertical(GUI.skin.box, GUILayout.Width(LcolumnWidth));

            if (GUILayout.Button("Load Preset"))
            {
                loadPresetWithDialog();
            }

            GUIStyle PresetStyle = new GUIStyle(EditorStyles.boldLabel);
            PresetStyle.normal.textColor = Color.green;
            PresetStyle.alignment = TextAnchor.MiddleCenter;
            EditorGUILayout.LabelField("Current Preset:", currentPresetName, PresetStyle);

            if (!checkBaseMesh)
            {
                GUIStyle warningStyle = new GUIStyle(EditorStyles.boldLabel);
                warningStyle.normal.textColor = Color.red;
                warningStyle.alignment = TextAnchor.MiddleCenter;
                EditorGUILayout.LabelField("You need to Set a Host Mesh Reference", warningStyle);
            }
            foldouts[13] = EditorGUILayout.BeginFoldoutHeaderGroup(foldouts[13], new GUIContent("Host Meshes",
                "The 3D mesh of the object that the ivy or climbing plants are growing on."));
            if (foldouts[13])
            {
                EditorGUILayout.LabelField("Selected Host Meshes", EditorStyles.boldLabel);

                // Scroll view for the list of selected meshes
                scrollPositionHost = EditorGUILayout.BeginScrollView(scrollPositionHost, GUILayout.Height(50));
                for (int i = 0; i < meshIO.selectedMeshes.Count; i++)
                {
                    EditorGUILayout.BeginHorizontal();
                    meshIO.selectedMeshes[i] = (GameObject)EditorGUILayout.ObjectField(meshIO.selectedMeshes[i], typeof(GameObject), true);
                    if (GUILayout.Button("Remove", GUILayout.Width(80)))
                    {
                        meshIO.selectedMeshes.RemoveAt(i);
                        i--;
                    }
                    EditorGUILayout.EndHorizontal();
                }
                EditorGUILayout.EndScrollView();

                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Drag and Drop Game Objects Here", EditorStyles.boldLabel);
                GUILayout.Box("", GUILayout.ExpandWidth(true), GUILayout.Height(1));
                Rect dropArea = GUILayoutUtility.GetRect(0.0f, 50.0f, GUILayout.ExpandWidth(true));
                GUI.Box(dropArea, "Drop Game Objects Here");
                IvyProUtility.CheckForDragAndDropHost(meshIO.selectedMeshes, dropArea);

                if (GUILayout.Button("Clear List"))
                {
                    for (int i = 0; i < meshIO.selectedMeshes.Count; i++)
                    {
                        meshIO.selectedMeshes.RemoveAt(i);
                        i--;
                    }
                }
                if (GUILayout.Button(new GUIContent("Set Host",
                    "Set the Selected Meshes as the Host: the 3D mesh surface that the climbing plants will grow on.")))
                {
                    if(meshIO.selectedMeshes.Count > 0)
                    {
                        string mFilePath = Path.Combine(permanentFolderPath , "Temp/BaseMesh.bin");
                        meshIO.SaveAs3DBin(meshIO.selectedMeshes, mFilePath, true);
                        checkBaseMesh = File.Exists(Path.Combine(permanentFolderPath, "Temp/BaseMesh.bin"));
                    }
                    else
                    {
                        Debug.Log("You need to add at least 1 mesh to the the list of Host Meshes");
                    }
                   
                }
            }
           
            EditorGUILayout.EndVertical();
            EditorGUILayout.BeginVertical(GUI.skin.box, GUILayout.Width(LcolumnWidth));
            EditorGUILayout.EndFoldoutHeaderGroup();
         
            if (GUILayout.Button(new GUIContent("Set Seed & GROW",
                            "Click this button and then select a location in your scene to place a seed. The plant will begin growing from this selected point"), GUILayout.Height(40)))
            {
                if (!checkBaseMesh)
                {
                    Debug.LogWarning("You need to Set a Host Mesh Reference first");
                }
                else
                {
                    // Subscribe to sceneview
                    SceneView.duringSceneGui += PlaceOneSeed;
                    placeSeeds = true;
                }
                        
            }

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button(new GUIContent("Set Seed & Target",
                "Click this button and then select two locations in your scene. The plant will start growing from the" +
                " first point and aim towards the second, guiding its growth initial direction. Then Click On Grow"), GUILayout.Height(40)))
            {
                SceneView.duringSceneGui += PlaceBridge;
                placeFirstPoint = true;
                placeSecondPoint = false;             
            }
            if (GUILayout.Button(new GUIContent("GROW",
                "Generate a new plant from the selected placement and direction."),GUILayout.Height(40)))
            {
                if (!checkBaseMesh)
                {
                    Debug.LogWarning("You need to Set a Host Mesh Reference first");
                }
                else
                {
                    if (seedTransform != null && targetTransform != null)
                    {
                        seed = seedTransform.position;
                        target = targetTransform.position;
                    }
                    makeIvyBridge();

                }
                
            }

            EditorGUILayout.EndHorizontal();
            foldouts[21] = EditorGUILayout.BeginFoldoutHeaderGroup(foldouts[21], new GUIContent("Seed & Target Info",
                " Attatch Game Objects to the Seed and Target points to set their positions manually without the need" +
                " of a mesh collider Or to visualize their location in the scene view"));
            if (foldouts[21])
            {
                ///// UNCOMMENT TO ALLOW SETTING THE SEED AND TARGET WITH EMPTY GAMEOBJECTS
                GUILayout.Label("Attatch to Seed", EditorStyles.boldLabel);

                Event evt = Event.current;
                Rect drop_area = GUILayoutUtility.GetRect(0.0f, 25.0f, GUILayout.ExpandWidth(true));
                GUI.Box(drop_area, seedTransform != null ? seedTransform.gameObject.name : "Drag & Drop Seed");
                switch (evt.type)
                {
                    case EventType.DragUpdated:
                    case EventType.DragPerform:
                        if (!drop_area.Contains(evt.mousePosition))
                            break;

                        DragAndDrop.visualMode = DragAndDropVisualMode.Copy;

                        if (evt.type == EventType.DragPerform)
                        {
                            DragAndDrop.AcceptDrag();

                            foreach (Object dragged_object in DragAndDrop.objectReferences)
                            {
                                // Check if we actually dragged a Transform
                                if (dragged_object is GameObject)
                                {
                                    GameObject temp = dragged_object as GameObject;
                                    seedTransform = temp.transform;

                                    seed = seedTransform.position;
                                    break;
                                }
                            }
                        }
                        break;
                }
                GUILayout.Label("Attatch to Target", EditorStyles.boldLabel);

                Event evtTarget = Event.current;
                Rect drop_areaTarget = GUILayoutUtility.GetRect(0.0f, 25.0f, GUILayout.ExpandWidth(true));
                GUI.Box(drop_areaTarget, targetTransform != null ? targetTransform.gameObject.name : "Drag & Drop Target");
                switch (evtTarget.type)
                {
                    case EventType.DragUpdated:
                    case EventType.DragPerform:
                        if (!drop_areaTarget.Contains(evt.mousePosition))
                            break;

                        DragAndDrop.visualMode = DragAndDropVisualMode.Copy;

                        if (evt.type == EventType.DragPerform)
                        {
                            DragAndDrop.AcceptDrag();

                            foreach (Object dragged_object in DragAndDrop.objectReferences)
                            {
                                // Check if we actually dragged a Transform
                                if (dragged_object is GameObject)
                                {
                                    GameObject temp = dragged_object as GameObject;
                                    targetTransform = temp.transform;

                                    target = targetTransform.position;
                                    break;
                                }
                            }
                        }
                        break;
                }
                /////

                seed = EditorGUILayout.Vector3Field("Seed Position:", seed);
                target = EditorGUILayout.Vector3Field("Target Position:", target);
                float distance = Vector3.Distance(seed, target);
                EditorGUILayout.LabelField("Distance:", distance.ToString());
            }
             


            EditorGUILayout.EndVertical();
            EditorGUILayout.EndHorizontal();

        }
        #endregion
        //**** OPTIMIZE **************************************
        #region 4.4) OPTIMIZATION
        private void DrawCleanUpTab()
        {
            float LcolumnWidth = Screen.width / (2 * dpiScaling);
            GUILayout.Label("OPTIMIZATION");
            ////
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.BeginVertical(GUI.skin.box, GUILayout.Width(LcolumnWidth));

            ////
            foldouts[15] = EditorGUILayout.BeginFoldoutHeaderGroup(foldouts[15], new GUIContent("Occlusion Pruner",
                "Drag and drop a collection of GameObjects and specify camera positions to remove occluded elements that aren't visible, improving scene performance."));
            if (foldouts[15])
            {
                GUILayout.Label("Upgrade to IvyPro to Access all the Settings ");
            }
            EditorGUILayout.EndFoldoutHeaderGroup();
            EditorGUILayout.EndVertical();
            EditorGUILayout.BeginVertical(GUI.skin.box, GUILayout.Width(LcolumnWidth));
            foldouts[16] = EditorGUILayout.BeginFoldoutHeaderGroup(foldouts[16], new GUIContent("Material-Based Mesh Combiner",
                "Drag and drop GameObjects to create new combined meshes based on shared materials"));
            if (foldouts[16])
            {

                GUILayout.Label("Upgrade to IvyPro to Access all the Settings ");

            }
            EditorGUILayout.EndFoldoutHeaderGroup();
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndHorizontal();

            foldouts[22] = EditorGUILayout.BeginFoldoutHeaderGroup(foldouts[22], new GUIContent("Utilities",
                "Buttons for cleaning temp files, change cache folder location, etc"));
            if (foldouts[22])
            {
                if (GUILayout.Button("Delete Ivy Meshes"))
                {
                    string MeshesFolderPath = Path.Combine(mainFolderPath, "IvyMeshes/");
                    IvyProUtility.DeleteFilesInFolder(MeshesFolderPath);
                }

                if (GUILayout.Button(new GUIContent("Purge Temporary Files",
                    "Click this button to clear all temporary files created during the plant generation process.")))
                {
                    meshIO.resetMeshcounter();
                    string meshesFolderPath = Path.Combine(mainFolderPath, "IvyMeshes/");
                    IvyProUtility.DeleteFilesInFolder(meshesFolderPath);
                    string occlusion = Path.Combine(permanentFolderPath, "Occlusion");
                    IvyProUtility.DeleteFilesInFolder(occlusion);
                    string temp = Path.Combine(permanentFolderPath, "Temp");
                    IvyProUtility.DeleteFilesInFolder(temp);
                    string export = Path.Combine(permanentFolderPath, "Export");
                    IvyProUtility.DeleteFilesInFolder(export);
                    checkBaseMesh = File.Exists(Path.Combine(permanentFolderPath, "Temp/BaseMesh.bin"));
                }
                
                if (GUILayout.Button(new GUIContent("Create Custom Cache Folder", "Select the folder where essential working temporary files like the host mesh, leaf scatter transforms, and list of occludable objects are stored.")))
                {
                    permanentFolderPath = EditorUtility.OpenFolderPanel("Select Cache", permanentFolderPath, "Cache");
                    IvyProUtility.CreateTempFolders(permanentFolderPath);
                    checkBaseMesh = File.Exists(Path.Combine(permanentFolderPath, "Temp/BaseMesh.bin"));
                    
                    Debug.Log("path: " + permanentFolderPath);
                }
              
            } 
        }
        #endregion
        #endregion
    }
}
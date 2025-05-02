using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Utility class containing a collection of static helper methods for various operations.
/// This class contains functions for file and path management, GUI creation, object validation and manipulation, and data interchange with external DLLs.
/// The methods assist in tasks such as retrieving script paths, managing preset data, initializing object counters, and preparing drag-and-drop areas in the Unity Editor.
/// </summary>
namespace IvyLite
{
    public struct Branch
    {
        public float radius;
        public Vector3[] points;

        public Branch(float radius, Vector3[] points)
        {
            this.radius = radius;
            this.points = points;
        }
    }
    public static class IvyProUtility
    {
        // Retrieves the path of a specified script within the Unity project.
        public static string GetScriptPath(string scriptName, bool relativePath = true)
        {
            string[] guids = AssetDatabase.FindAssets(scriptName + " t:script");
            if (guids.Length == 0)
            {
                Debug.LogError(scriptName + ".cs not found!");
                return string.Empty;
            }

            string path = AssetDatabase.GUIDToAssetPath(guids[0]);
            // Get directory path
            string directoryPath = Path.GetDirectoryName(path);

            // If the directoryPath ends with "/Editor", move one directory up.
            if (directoryPath.EndsWith("Editor"))
            {
                directoryPath = Path.GetDirectoryName(directoryPath);
            }
            else
            {
                Debug.LogError("This Addon expects IvyEditorWindow.cs to be placed inside the Editor Subfolder and finds relative paths to other assets from its locations");
            }

            if (!relativePath)
            {
                directoryPath = Path.GetFullPath(directoryPath);
            }

            return directoryPath;
        }

        // Removes all files within a given folder.
        public static void DeleteFilesInFolder(string thisFolderPath)
        {
            // Check if the directory exists
            if (Directory.Exists(thisFolderPath))
            {
                // Get all files in the directory
                string[] files = Directory.GetFiles(thisFolderPath);

                // Loop through each file and delete it
                foreach (string file in files)
                {
                    File.Delete(file);
                }
                AssetDatabase.Refresh();
            }
            else
            {
                Debug.LogWarning("Directory does not exist: " + thisFolderPath);
            }
        }

        // Gets the path to the asset whether or not is a Prefab.
        public static string GetAssetPathOrPrefabPath(GameObject go)
        {
            string assetPath = AssetDatabase.GetAssetPath(go);
            if (string.IsNullOrEmpty(assetPath))
            {
                GameObject prefab = PrefabUtility.GetCorrespondingObjectFromSource(go);
                if (prefab != null)
                {
                    assetPath = AssetDatabase.GetAssetPath(prefab);
                }
            }

            return assetPath;
        }
        // Reads and populates a float array containing Setting Sliders from a file.
        public static void ReadFloatsFromBinaryFile(string thisPath, float[] outputArray)
        {
            using (FileStream fs = new FileStream(thisPath, FileMode.Open))
            {
                using (BinaryReader br = new BinaryReader(fs))
                {
                    int index = 0;
                    while (fs.Position < fs.Length && index < outputArray.Length)
                    {
                        outputArray[index] = br.ReadSingle();
                        index++;
                    }
                }
            }
        }
       

     
        // Sets up a draggable rectangle zone for objects to be processed and added to a list.
        public static void CheckForDragAndDropHost(List<GameObject> ListOfGameObjects, Rect dropArea)
        {
            Event currentEvent = Event.current;
            if (!dropArea.Contains(currentEvent.mousePosition))
                return;

            switch (currentEvent.type)
            {
                case EventType.DragUpdated:
                case EventType.DragPerform:
                    DragAndDrop.visualMode = DragAndDropVisualMode.Copy;

                    if (currentEvent.type == EventType.DragPerform)
                    {
                        DragAndDrop.AcceptDrag();

                        foreach (var draggedObject in DragAndDrop.objectReferences)
                        {
                            GameObject draggedGameObject = draggedObject as GameObject;
                            if (draggedGameObject != null)
                            {
                                AddMeshObjectsToList(draggedGameObject, ListOfGameObjects);
                            }
                        }
                    }
                    break;
            }
        }

        public static void CheckForDragAndDrop(List<GameObject> ListOfGameObjects, Rect dropArea)
        {
            Event currentEvent = Event.current;
            if (!dropArea.Contains(currentEvent.mousePosition))
                return;

            switch (currentEvent.type)
            {
                case EventType.DragUpdated:
                case EventType.DragPerform:
                    DragAndDrop.visualMode = DragAndDropVisualMode.Copy;

                    if (currentEvent.type == EventType.DragPerform)
                    {
                        DragAndDrop.AcceptDrag();

                        foreach (var draggedObject in DragAndDrop.objectReferences)
                        {
                            GameObject draggedGameObject = draggedObject as GameObject;
                            if (draggedGameObject != null)
                            {
                                // Add the GameObject if it has a MeshFilter
                                if (draggedGameObject.GetComponent<MeshFilter>() != null || draggedGameObject.GetComponentInChildren<MeshFilter>() != null)
                                {
                                    ListOfGameObjects.Add(draggedGameObject);
                                }
                            }
                        }
                    }
                    break;
            }
        }
        // Recursively scans for objects with mesh filter components, adding them (and their children) to a list.
        public static void AddMeshObjectsToList(GameObject gameObject, List<GameObject> ListOfGameObjects)
        {
            // Add the GameObject if it has a MeshFilter
            if (gameObject.GetComponent<MeshFilter>() != null)
            {
                ListOfGameObjects.Add(gameObject);
            }

            // Recursively process the children
            foreach (Transform child in gameObject.transform)
            {
                AddMeshObjectsToList(child.gameObject, ListOfGameObjects);
            }
        }
      

        // Creates temporary directories for data interchange between Unity and external DLL functions.
        public static void CreateTempFolders(string ivyProTemp)
        {
            // If they don't exist, create folders
            if (!Directory.Exists(ivyProTemp))
            {
                Directory.CreateDirectory(ivyProTemp);
            }
            string temp = Path.Combine(ivyProTemp, "Temp");
            if (!Directory.Exists(temp))
            {
                Directory.CreateDirectory(temp);
            }
            string export = Path.Combine(ivyProTemp, "Export");
            if (!Directory.Exists(export))
            {
                Directory.CreateDirectory(export);
            }
            string occlusion = Path.Combine(ivyProTemp, "Occlusion");
            if (!Directory.Exists(occlusion))
            {
                Directory.CreateDirectory(occlusion);
            }
        }
        // Creates IvyMesh folder for Mesh Assets
        public static void CreateIvyMeshFolder(string mainFolder)
        {
            string ivyMeshesPath = Path.Combine(mainFolder, "IvyMeshes");
            // If they don't exist, create folder
            if (!Directory.Exists(ivyMeshesPath))
            {
                Directory.CreateDirectory(ivyMeshesPath);
            }
           
        }

        // Initializes the counter for GameObjects named "IvyLite_".
        public static int InitializeIvyCount()
        {
            GameObject[] allObjects = GameObject.FindObjectsOfType<GameObject>();

            int maxNumber = 0;

            foreach (GameObject obj in allObjects)
            {
                if (obj.name.StartsWith("IvyLite_"))
                {
                    string numberPart = obj.name.Substring(3);

                    if (int.TryParse(numberPart, out int number))
                    {
                        maxNumber = Mathf.Max(maxNumber, number);
                    }
                }
            }
            return maxNumber;
        }
        public static void FixPaths(string mainFolderPath)
        {
            string presetsFolder = Path.Combine(mainFolderPath, "Presets/");
            Debug.Log(presetsFolder);

            if (!Directory.Exists(presetsFolder))
            {
                Debug.LogError("Presets folder not found: " + presetsFolder);
                return;
            }

            string[] allPresetPaths = Directory.GetFiles(presetsFolder, "*.json", SearchOption.AllDirectories);

            foreach (string presetPath in allPresetPaths)
            {
                if (Path.GetFileName(presetPath).StartsWith("."))
                {
                    continue; // Skip files starting with a dot
                }

                string jsonContent = File.ReadAllText(presetPath);

                // Use a regex to find all quoted strings in the JSON file
                var matches = Regex.Matches(jsonContent, "\"(.*?)\"");
                foreach (Match match in matches)
                {
                    string foundString = match.Groups[1].Value;

                    if (foundString.Contains("IvyLiteData/"))
                    {
                        // Replace everything before the last occurrence of "IvyProData/" with mainFolderPath
                        int index = foundString.LastIndexOf("IvyLiteData/");
                        string newPath = Path.Combine(mainFolderPath, foundString.Substring(index + "IvyLiteData/".Length)); // Skip "IvyProData/"
                        jsonContent = jsonContent.Replace("/" + foundString + "", "" + newPath + "/");
                    }
                }

                File.WriteAllText(presetPath, jsonContent);
            }

            AssetDatabase.Refresh();
            EditorUtility.DisplayDialog("Success", "Converted paths in presets!", "OK");
        }


        // "Fix Presets Paths", "Corrects the paths in presets json files replacing the parts where it said IvyPro by IvyLite to ensure they point to the correct locations."
        public static void FixPathsProToLite(string mainFolderPath)
        {
            string presetsFolder = Path.Combine(mainFolderPath, "Presets/");
            Debug.Log(presetsFolder);

            if (!Directory.Exists(presetsFolder))
            {
                Debug.LogError("Presets folder not found: " + presetsFolder);
                return;
            }

            string[] allPresetPaths = Directory.GetFiles(presetsFolder, "*.json", SearchOption.AllDirectories);

            foreach (string presetPath in allPresetPaths)
            {
                if (Path.GetFileName(presetPath).StartsWith("."))
                {
                    continue; // Skip files starting with a dot
                }

                string jsonContent = File.ReadAllText(presetPath);

                // Replace all instances of 'IvyPro' with 'IvyLite' in the JSON content
                jsonContent = jsonContent.Replace("IvyPro", "IvyLite");

                File.WriteAllText(presetPath, jsonContent);
            }

            AssetDatabase.Refresh();
            EditorUtility.DisplayDialog("Success", "Converted paths in presets!", "OK");
        }



        public static void RemoveMacFiles()
        {
            string directory = EditorUtility.OpenFolderPanel("Select a folder to clean", "", "");

            // If the user cancels folder selection, the returned path will be empty.
            if (string.IsNullOrEmpty(directory))
                return;

            RemoveMacFilesFromDirectory(directory);
            //AssetDatabase.Refresh(); // Refresh the Unity Editor's asset database.
        }

        private static void RemoveMacFilesFromDirectory(string directory)
        {
            if (!Directory.Exists(directory))
            {
                Debug.LogError($"Directory {directory} does not exist!");
                return;
            }

            string[] files = Directory.GetFiles(directory);
            foreach (var file in files)
            {
                if (Path.GetFileName(file).StartsWith("._"))
                {
                    try
                    {
                        File.Delete(file);
                        Debug.Log($"Deleted: {file}");
                    }
                    catch (System.Exception e)
                    {
                        Debug.LogError($"Failed to delete {file}. Reason: {e.Message}");
                    }
                }
            }

            // Recursive part: Checking subdirectories.
            string[] subdirectories = Directory.GetDirectories(directory);
            foreach (var subdirectory in subdirectories)
            {
                RemoveMacFilesFromDirectory(subdirectory);
            }
        }



        // ReadIvyFile : Reads a file containing,all the indormation about the IvyMesh but stores only the nodes positions of every branch
        // it return a List of the branches, each branch containing a Vector3 array
        private static List<Branch> ReadIvyFile(string filePath)
        {
            List<Branch> lines = new List<Branch>();
            using (BinaryReader reader = new BinaryReader(File.Open(filePath, FileMode.Open)))
            {
                if (!File.Exists(filePath))
                {
                    Debug.LogError("File not found: " + filePath);
                    return null; // File not found
                }

                // Read header 
                //int jump = 16;
                //reader.BaseStream.Seek(jump, SeekOrigin.Begin);
                reader.ReadBytes(16); // Skip header
                // Read Roots, if needed, in this case it's always 1 because there is only one seed.
                uint roots = reader.ReadUInt32();
                // Read number of Ramas
                uint numRamas = reader.ReadUInt32();

                for (int i = 0; i < numRamas; i++)
                {
                    // Read Rama ID and Node ID, Not used
                    int ramaID = reader.ReadInt32();
                    int nodeID = reader.ReadInt32();
                    // Read number of Nodes
                    int numNodes = reader.ReadInt32();
                    Vector3[] nodes = new Vector3[numNodes];

                    float radius = 0;
                    for (int j = 0; j < numNodes; j++)
                    {
                        float x = reader.ReadSingle();
                        float y = reader.ReadSingle();
                        float z = reader.ReadSingle();

                        if (j == 0) // Read radius of the first node
                        {
                            radius = reader.ReadSingle();
                            reader.BaseStream.Position += sizeof(float) * 6 + sizeof(int);
                        }
                        else
                        {
                            reader.BaseStream.Position += sizeof(float) * 7 + sizeof(int);
                        }
                        nodes[j] = new Vector3(x, y, z);                        
                        
                    }

                    lines.Add(new Branch(radius, nodes));
                }
            }
            if (lines.Count == 0)
            {
                Debug.LogWarning("No data found in Ivy file: ");
                return null;
            }
            return lines;
        }
        //Create Lines using a Line Renderer Component from the points in a .Ivy file
        public static void CreateLines(string filePath,
            GameObject parentObject,
            Material lineMaterial,
            float lineWidthMultiplier,
            float fadeRadiusFC,
            float simplifyFC)
        {
            List<Branch> lines = ReadIvyFile(filePath);

            if (lines == null || lines.Count == 0)
            {
                Debug.LogError("No branch data found or error reading file.");
                return;
            }

            foreach (Branch branch in lines)
            {
                GameObject lineObject = new GameObject("BranchLine");
                LineRenderer lineRenderer = lineObject.AddComponent<LineRenderer>();

                // Configure the LineRenderer
                lineRenderer.material = lineMaterial;
                // loop false is the default so not really necessary
                lineRenderer.loop = false;
                lineRenderer.textureMode = LineTextureMode.Stretch;
                // textureScale doesn?t exist in unity2021 but does in 2022
                #if UNITY_2022_1_OR_NEWER
                lineRenderer.textureScale = new Vector2(10.0f, 1.0f);
                #endif
                lineRenderer.startWidth = branch.radius;
                //Geometric Progression to calculate the endwidth 
                lineRenderer.endWidth = branch.radius * Mathf.Pow(fadeRadiusFC, branch.points.Length - 1);                
                lineRenderer.positionCount = branch.points.Length;
                lineRenderer.SetPositions(branch.points);
                lineRenderer.widthMultiplier = lineWidthMultiplier;
                if (simplifyFC > 0.001f)
                {
                    lineRenderer.Simplify(simplifyFC);
                }                
                // Parent the branch to the specified parent object
                lineObject.transform.parent = parentObject.transform;
            }
        }
    }
}
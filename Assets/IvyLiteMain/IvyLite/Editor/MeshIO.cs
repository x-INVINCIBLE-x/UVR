using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using System.Runtime.InteropServices;
using System.Linq;

namespace IvyLite
{
    /// <summary>
    /// Class that Groups variables and methods that deal with the creation, reading and saving of assets with a MeshFilter component.
    /// </summary>
    /// 
    public class MeshIO
    {
        private int meshCounter;
        private string mainFolderPath;
        public List<GameObject> selectedIvys;
        public List<GameObject> selectedMeshes;
        public bool bigMesh;
        public bool saveMesh;

        public MeshIO(string myMainFolderPath)
        {
            meshCounter = 0;
            this.mainFolderPath = myMainFolderPath;
            selectedIvys = new List<GameObject>();
            selectedMeshes = new List<GameObject>();
            bigMesh = false;
            saveMesh = true;
            
        }
        public void resetMeshcounter()
        {
            meshCounter = 0;
        }
        public void UpdateMeshCounter()
        {
            meshCounter = 0;
            string MeshesFolderPath = Path.Combine(mainFolderPath, "IvyMeshes/");
            string[] files = Directory.GetFiles(MeshesFolderPath, "mesh*.asset");
            foreach (string file in files)
            {
                string fileName = Path.GetFileNameWithoutExtension(file);
                if (fileName.StartsWith("mesh"))
                {
                    int fileNumber;
                    if (int.TryParse(fileName.Substring(4), out fileNumber))
                    {
                        meshCounter = Mathf.Max(meshCounter, fileNumber + 1);
                    }
                }
            }
        }
        /// <summary>
        /// Reads a Binary file with Vertices and Indices and Generates a New Mesh asset.
        /// Then it creates a new GameObject and add a meshfilter component that points to that Mesh asset.
        /// </summary>
        /// <param name="thisPath"> Path to the file</param>
        /// <param name="myMaterial"> Material Applied to the new GameObject</param>
        /// <param name="parentObject"> The GameObject that will be parent of the created GameObject </param>
        /// <returns> Created GameObject with a meshfilter component </returns>
        public GameObject ImportIvyMesh(string thisPath, Material myMaterial, GameObject parentObject)
        {
            GameObject meshObject;
            using (BinaryReader reader = new BinaryReader(File.Open(thisPath, FileMode.Open)))
            {
                // Read the number of vertices
                int numVertices = reader.ReadInt32();
                Vector3[] vertices = new Vector3[numVertices];
                // Read the vertices
                byte[] vertexBytes = reader.ReadBytes(numVertices * 3 * sizeof(float));
                GCHandle vertexHandle = GCHandle.Alloc(vertices, GCHandleType.Pinned);
                Marshal.Copy(vertexBytes, 0, vertexHandle.AddrOfPinnedObject(), vertexBytes.Length);
                vertexHandle.Free();
    
                // Read the number of triangles
                int numTriangles = reader.ReadInt32();
                int[] triangles = new int[numTriangles * 3];
    
                // Read the triangle indices
                byte[] triangleBytes = reader.ReadBytes(numTriangles * 3 * sizeof(int));
                GCHandle triangleHandle = GCHandle.Alloc(triangles, GCHandleType.Pinned);
                Marshal.Copy(triangleBytes, 0, triangleHandle.AddrOfPinnedObject(), triangleBytes.Length);
                triangleHandle.Free();
    
                // Read the number of UVs
                int numUVs = reader.ReadInt32();
                
                Vector2[] uvs = new Vector2[numUVs];
    
                // Read the UV coordinates
                
                byte[] uvBytes = reader.ReadBytes(numUVs * 2  * sizeof(float));
                GCHandle uvHandle = GCHandle.Alloc(uvs, GCHandleType.Pinned);
                Marshal.Copy(uvBytes, 0, uvHandle.AddrOfPinnedObject(), uvBytes.Length);
                uvHandle.Free();
    
                Mesh mesh = new Mesh();
                if (numVertices > 65535)
                {
                    mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
                }
                mesh.SetVertices(vertices);
                mesh.SetTriangles(triangles, 0);
                mesh.SetUVs(0, uvs);
                mesh.RecalculateNormals();
                mesh.RecalculateTangents();
                mesh.RecalculateBounds();             
                // Save the mesh as an asset in the specified folder
                string MeshesFolderPath = Path.Combine(mainFolderPath, "IvyMeshes/");
                string assetFileName = string.Format("mesh{0:D3}.asset", meshCounter);
                string assetFilePath = Path.Combine(MeshesFolderPath, assetFileName);
                AssetDatabase.CreateAsset(mesh, assetFilePath);
                AssetDatabase.SaveAssets();
                //AssetDatabase.Refresh();    
                Debug.LogFormat("Mesh saved at {0}", assetFilePath);
    
                meshObject = new GameObject("ivy");
                // Add and the MeshFilter component
                MeshFilter meshFilter = meshObject.AddComponent<MeshFilter>();
                meshFilter.sharedMesh = mesh;
                // Add and Get the Renderer component of the target GameObject
                Renderer renderer = meshObject.AddComponent<MeshRenderer>();
                // Make the current Asset a child of the specified GameObject 
                meshObject.transform.SetParent(parentObject.transform);
                // Load the material from the Assets folder                
                // Assign the material to the Renderer's material property
                renderer.material = myMaterial;
                // place item in viewport
                if (meshObject != null)
                {
                    meshObject.transform.position = Vector3.zero;
                    meshObject.transform.rotation = Quaternion.identity;
                    meshObject.transform.localScale = Vector3.one;
                }
                
                meshCounter++;           
            }
            return meshObject;
        }
        /// <summary>
        /// Writes a Binary file containing the Bounds of each of the GameObjects contained in a List
        /// </summary>
        /// <param name="ListOfGameObjects"> List of Game Objects we want to export the bouding boxes </param>
        /// <param name="outputPath"> Path for created file</param>

        public void ExportBoundingBoxes(List<GameObject> ListOfGameObjects, string outputPath)
         {
            using (BinaryWriter writer = new BinaryWriter(File.Open(outputPath, FileMode.Create)))
            {
                // Write the number of meshes
                writer.Write(ListOfGameObjects.Count);
        
                foreach (GameObject meshObj in ListOfGameObjects)
                {
                    // Get the bounding box
                    MeshRenderer[] meshes = meshObj.GetComponentsInChildren<MeshRenderer>();
                    Bounds bounds = meshes[0].bounds;
                    for (int j = 1; j < meshes.Length; j++)
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
                }
            }
         }


        // Take a list of Game Objects with a MeshFilter component and combine it to a single 3d mesh ans saves them in a file
        // the custom 3D mesh file type starts with the number of vertices , the number of triangles , the list of floats
        // represening the X,Y,Z coordinates and the list of triangles each one containing the index of each of the 3 vertices
        public void SaveAs3DBin(List<GameObject> ListOfGameObjects,string thisPath ,bool TransformIt)
        {        
            
            using (BinaryWriter writer = new BinaryWriter(File.Open(thisPath, FileMode.Create)))
            {
                List<Vector3> combinedVertices = new List<Vector3>();
                List<int> combinedTriangles = new List<int>();
    
                foreach (GameObject go in ListOfGameObjects)
                {
                    Mesh mesh = go.GetComponent<MeshFilter>().sharedMesh;
                    if (TransformIt)
                    {
                        AddMeshDataTransform(mesh, go.transform ,ref combinedVertices, ref combinedTriangles);
                    }                
                    else
                    {
                        AddMeshData(mesh, ref combinedVertices, ref combinedTriangles);
                    }
                }
    
                writer.Write((uint)combinedVertices.Count);
                for (int i = 0; i < combinedVertices.Count; i++)
                {
                    writer.Write(combinedVertices[i].x);
                    writer.Write(combinedVertices[i].y);
                    writer.Write(combinedVertices[i].z);
                }
    
                writer.Write((uint)combinedTriangles.Count / 3);
                for (int i = 0; i < combinedTriangles.Count; i++)
                {
                    writer.Write(combinedTriangles[i]);
                }
            }
        }

        // Adds the vertices of every GameObject in the game object list into one single list of vertices
        private void AddMeshData(Mesh mesh, ref List<Vector3> combinedVertices, ref List<int> combinedTriangles)
        {
            int vertexOffset = combinedVertices.Count;
    
            combinedVertices.AddRange(mesh.vertices);

            combinedTriangles.AddRange(mesh.triangles.Select(t => t + vertexOffset));
        }

        // Adds the vertices of each Game Object in a GameObject list , Apply a transform
        // and store them into a unique list of vertices and triangles Indices for the new mesh.
        private void AddMeshDataTransform(Mesh mesh, Transform transform, ref List<Vector3> combinedVertices, ref List<int> combinedTriangles)
        {
            int vertexOffset = combinedVertices.Count;
            combinedVertices.AddRange(mesh.vertices.Select(v => transform.TransformPoint(v)));
            combinedTriangles.AddRange(mesh.triangles.Select(t => t + vertexOffset));

        }       

        // The same as the one before but also storing the Normals
        private void AddMeshDataTransform(Mesh mesh, Transform transform, ref List<Vector3> combinedVertices, ref List<int> combinedTriangles, ref List<Vector2> combinedUVs, ref List<Vector3> combinedNormals)
        {
            int vertexOffset = combinedVertices.Count;
            combinedVertices.AddRange(mesh.vertices.Select(v => transform.TransformPoint(v)));
            combinedTriangles.AddRange(mesh.triangles.Select(t => t + vertexOffset));
            combinedUVs.AddRange(mesh.uv); 
            combinedNormals.AddRange(mesh.normals);
        }


        // Recursive Function that takes a GameObject that is a parent, grandparent , etc...
        // adds them to a list of gameobject with a meshcomponent and, if they have a material, adds that material to a Set of materials
        private void AddMeshesToList(GameObject parent, List<GameObject> ListOfGameObjects, HashSet<Material> materialSet)
        {
                if (parent == null || ListOfGameObjects == null)
                {
                    Debug.LogError("Parent or List is null.");
                    return;
                }
        
                foreach (Transform child in parent.transform)
                {
                    if (child.gameObject.GetComponent<MeshFilter>() != null)
                    {
                        ListOfGameObjects.Add(child.gameObject);
                    
                        Material mat = child.gameObject.GetComponent<MeshRenderer>().sharedMaterial;
                        if (mat != null)
                        {
                            materialSet.Add(mat);
                        }
                    }
                
                    AddMeshesToList(child.gameObject, ListOfGameObjects, materialSet);
                }
        } 
        
    }
}
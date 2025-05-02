using UnityEngine;
using System.IO;
using UnityEditor;
using System.Collections.Generic;

namespace IvyLite
{
    /// <summary>
    /// Class containing Preset Data and functions for reading and saving presets.
    /// </summary>
    /// 
    [System.Serializable]
    public struct GranularParams
    {
        public int count;
        public float angleSlope;
        public float angleVariation;
        public float scale;
        public float scaleVariation;
        public float easing;
        public float minSize;
        public float minAngle;
        public float maxAngle;
        public float proximal;
        public float distal;
        public float minDistance;
        public bool onMain;
        public bool onBranch;
        public bool onCatenary;
        public bool onAir;
        public bool hostNormal;
        public float maxDistance;
        public float normalMin;
        public float normalMax;
        public float normalAngleVariation;
        public float surfaceOffset;

        public static GranularParams Initialize()
        {
            GranularParams granularItem = new GranularParams();
            granularItem.angleSlope = 0.0f;
            granularItem.angleVariation = 10.0f;
            granularItem.scale = 1.0f;
            granularItem.scaleVariation = 0.0f;
            granularItem.easing = 0.80f;
            granularItem.minSize = 0.01f;
            granularItem.minAngle = 70.0f;
            granularItem.maxAngle = 90.0f;
            granularItem.proximal = 0.1f;
            granularItem.distal = 0.9f;
            granularItem.minDistance = 0.15f;
            granularItem.onMain = false;
            granularItem.onBranch = true;
            granularItem.onCatenary = true;
            granularItem.onAir = true;
            granularItem.hostNormal = false;
            granularItem.maxDistance = 0.1f;
            granularItem.normalMin = 70.0f;
            granularItem.normalMax = 90.0f;
            granularItem.normalAngleVariation = 36.0f;
            granularItem.surfaceOffset = 0.0f;
            return granularItem;
        }
    }
    [System.Serializable]
    public class IvyPreset
    {
        // I can't rename Fruits as Granular because the Preset I made could stop working
        //TO DO ... Change the presets so I can rename Fruit with Granular and be consistent with naming...
        // Paths for Stems, Leaves, fruits and endings
        public List<string> _StemPaths;
        public List<string> _LeavesPaths;
        public List<string> _EndingsPaths;
        public List<string> _FruitsPaths;
        // A float array that fills all the Ivy Parameters.
        public float[] Prsliders;
        public string PresetName;
        public List<GranularParams> _fruitSettins;      
        public string _IvyMaterialPath;
        // Constructor for the class
        public IvyPreset()
        {
            _StemPaths = new List<string>();
            _LeavesPaths = new List<string>();
            _EndingsPaths = new List<string>();
            _FruitsPaths = new List<string>();
            Prsliders = new float[120];
            PresetName = "Default";
            _fruitSettins = new List<GranularParams>();
        }
    }

    public class PresetManager
    {
        private IvyPreset preset;
        private string PresetFolderPath;

        public PresetManager(IvyPreset preset, string PresetFolderPath )
        {
            this.preset = preset;
            this.PresetFolderPath = PresetFolderPath;
        }

        // Method to save the current preset to a file
        public void SavePreset()
        {            
            string path = EditorUtility.SaveFilePanel("Save Ivy Preset", this.PresetFolderPath, "IvyPreset.json", "json");
            
            if (path.Length != 0)
            {
                this.preset.PresetName = System.IO.Path.GetFileNameWithoutExtension(path);
                // Convert the preset object to JSON
                string jsonString = JsonUtility.ToJson(this.preset, true);

                // Write the JSON string to a file
                File.WriteAllText(path, jsonString);            

                // Refresh the AssetDatabase to include the newly created file
                AssetDatabase.Refresh();
            }
        }

    // Method to load a preset from a file
    public bool LoadPreset()
        {
            string path = EditorUtility.OpenFilePanel("Load Ivy Preset", this.PresetFolderPath, "json");
            if (path.Length != 0)
            {
                this.preset.PresetName = System.IO.Path.GetFileNameWithoutExtension(path);
                string jsonString = File.ReadAllText(path);
                IvyPreset loadedPreset = JsonUtility.FromJson<IvyPreset>(jsonString);
                
                this.preset._StemPaths = loadedPreset._StemPaths;
                this.preset._LeavesPaths = loadedPreset._LeavesPaths;
                this.preset._EndingsPaths = loadedPreset._EndingsPaths;
                this.preset._FruitsPaths = loadedPreset._FruitsPaths;
                this.preset.Prsliders = loadedPreset.Prsliders;
                this.preset._fruitSettins = loadedPreset._fruitSettins;
                this.preset._IvyMaterialPath = loadedPreset._IvyMaterialPath;
                return true;
            }
            return false;
        }
        public bool LoadPresetWithPath(string path)
        {            
            if (path.Length != 0)
            {
                this.preset.PresetName = System.IO.Path.GetFileNameWithoutExtension(path);
                string jsonString = File.ReadAllText(path);
                IvyPreset loadedPreset = JsonUtility.FromJson<IvyPreset>(jsonString);
                
                this.preset._StemPaths = loadedPreset._StemPaths;
                this.preset._LeavesPaths = loadedPreset._LeavesPaths;
                this.preset._EndingsPaths = loadedPreset._EndingsPaths;
                this.preset._FruitsPaths = loadedPreset._FruitsPaths;
                this.preset.Prsliders = loadedPreset.Prsliders;
                this.preset._fruitSettins = loadedPreset._fruitSettins;
                this.preset._IvyMaterialPath = loadedPreset._IvyMaterialPath;
                return true;
            }
            return false;
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.AI.Navigation;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Climbing;

[System.Serializable]
public class WeightedPrefab { public GameObject prefab; public float weight = 1f; }
public enum FormationTypeGround { Crater, Sine, CustomWave }
public enum WaveAxis { X, Z, Both }

[System.Serializable]
public class RadiusBasedPrefab
{
    public float radius; // Outer bound of ring
    public Vector3 centerOffset = Vector3.zero;
    public GameObject centerElement;
    public float centralElementRadius = 20f;
    public float centerYOffset = 0f;
    public List<WeightedPrefab> prefabs; // Prefabs valid for this radius
}

[System.Serializable]
public class CustomWaveSettings
{
    public AnimationCurve curve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    public float amplitude = 2f;
    public WaveAxis axis = WaveAxis.X;
}

[System.Serializable]
public class FormationConfig
{
    public FormationTypeGround type;
    public bool jitterX = true;
    public bool jitterY = true;
}

[System.Serializable]
public class FormationContainer
{
    public int difficultyLevel;
    public List<FormationConfig> config;
}

[RequireComponent(typeof(NavMeshSurface))]
public class GridFormationController : FormationProvider
{
    [SerializeField] private bool drawGizmos;
    [SerializeField] private bool deactivateOnSpawn = false;
    [SerializeField] private int spawnPerFrame = 5;
    [SerializeField] private GridFormationData saveToSO;

    [SerializeField] private GridFormationDatabase gridDatabase;
    [SerializeField] private string groupKey = "Default";

    [Header("Radius Based Prefabs")]
    public List<RadiusBasedPrefab> radiusPrefabs = new List<RadiusBasedPrefab>();
    [Header("Fallback Corner Prefabs")]
    public List<WeightedPrefab> cornerPrefabs = new List<WeightedPrefab>();
    [Header("Prefabs & Weights")] public List<WeightedPrefab> cuboidPrefabs;
    [Header("Grid Settings")] public int gridWidth = 20, gridDepth = 20;
    [Header("Snapping Settings")]
    public float snapGap = 2f;  // adjustable gap between consecutive blocks
    public Vector3 snapDirection = Vector3.right;
    [Header("Spacing (min/max)")] public float minSpacing = 1f, maxSpacing = 3f;
    [Header("Density")][Range(0f, 1f)] public float density = 1f;
    [Header("Max Jitter XZ")] public float maxJitterXZ = 0.5f;
    [Header("Max Jitter Y")] public float maxJitterY = 1f;
    [Header("Crater")] public float innerRadius = 5f, outerRadius = 10f, maxDepth = 3f;
    [Header("Sine")] public float sineAmplitude = 2f, sineFrequency = 2f;
    [Header("Custom Wave")] public List<CustomWaveSettings> customWaveSettings = new List<CustomWaveSettings>();
    [Header("Formation Sequence")]
    public List<FormationContainer> formations = new List<FormationContainer>();
    public float transitionDuration = 3f;

    private List<Transform> instances = new List<Transform>();
    private List<List<Vector3>> positionsPerFormation = new List<List<Vector3>>();
    private List<Vector3> transitionStart = new List<Vector3>();
    private List<Vector3> transitionTarget = new List<Vector3>();
    private List<Quaternion> savedRotations = null;

    private NavMeshSurface[] navMeshSurfaces;
    private Vector3 gridCenter;
    private float gridSpanX, gridSpanZ;
    private float timer = 0f;
    private bool isTransitioning = false;
    private bool isExactRestore = false;
    private int difficultyLevel = 0;
    private int currentIndex = 0;

#if UNITY_EDITOR
    private void OnValidate()
    {
        for (int i = 0; i < formations.Count; i++)
        {
            formations[i].difficultyLevel = i + 1;
        }
    }
#endif
#if UNITY_EDITOR
    [ContextMenu("Editor: Generate Initial Grid and Spawn")]
    private void Editor_GenerateInitialGridAndSpawn()
    {
        if (Application.isPlaying)
        {
            Debug.LogWarning("Use this only in Edit Mode.");
            return;
        }
        difficultyLevel = 0; // Base formation at index 0
        Editor_DeleteSpawnedGrid();
        InitializeFormations();

        if (positionsPerFormation.Count == 0 || positionsPerFormation[0].Count == 0)
        {
            Debug.LogError("Failed to initialize base formation.");
            return;
        }

        StartCoroutine(SpawnFormation(0));
        Debug.Log("Initial formation spawned. You may now reposition the blocks.");
    }

    [ContextMenu("Editor: Stop Procedural Generation")]
    private void StopGeneration()
    {
        StopAllCoroutines();
    }

    [ContextMenu("Editor: Save Current Positions to Formation[0]")]
    private void Editor_SaveCurrentPositionsAsBaseFormation()
    {
        if (Application.isPlaying)
        {
            Debug.LogWarning("Use this only in Edit Mode.");
            return;
        }

        if (instances == null || instances.Count == 0)
        {
            Debug.LogError("No instances found in scene to save.");
            return;
        }

        if (positionsPerFormation.Count == 0)
            positionsPerFormation.Add(new List<Vector3>());

        positionsPerFormation[0] = new List<Vector3>();
        foreach (var inst in instances)
        {
            positionsPerFormation[0].Add(inst.localPosition);
        }
        Debug.Log(positionsPerFormation[0].Count + " positions saved to Formation[0].");
        Debug.Log("Saved current positions as base spawn formation at index 0. Amount: " + instances.Count);
    }

    [ContextMenu("Editor: Delete Spawned Grid")]
    private void Editor_DeleteSpawnedGrid()
    {
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            GameObject.DestroyImmediate(transform.GetChild(i).gameObject);
        }
        instances.Clear();
        positionsPerFormation.Clear();
        Debug.Log("Cleared grid.");
    }

    [ContextMenu("Editor: Export Children to ScriptableObject")]
    private void Editor_ExportToScriptableObject()
    {
#if UNITY_EDITOR
        string path = "Assets/Data/Grid Data/Grid Position Data";

        if (!System.IO.Directory.Exists(path))
            System.IO.Directory.CreateDirectory(path);

        // Check if existing SO is valid and matches GameObject name
        if (saveToSO == null || !saveToSO.name.StartsWith(gameObject.name))
        {
            // Create a new ScriptableObject if null OR name mismatch
            saveToSO = ScriptableObject.CreateInstance<GridFormationData>();

            string assetPathAndName = UnityEditor.AssetDatabase.GenerateUniqueAssetPath(
                path + "/" + gameObject.name + " Positions.asset"
            );

            UnityEditor.AssetDatabase.CreateAsset(saveToSO, assetPathAndName);
            UnityEditor.AssetDatabase.SaveAssets();

            Debug.Log("Created new ScriptableObject at: " + assetPathAndName);
        }

        if (transform.childCount == 0)
        {
            Debug.LogError("No children under this GameObject to export.");
            return;
        }

        saveToSO.positions = new List<Vector3>();
        saveToSO.rotations = new List<Quaternion>();
        saveToSO.prefabs = new List<GameObject>();

        for (int i = 0; i < transform.childCount; i++)
        {
            Transform child = transform.GetChild(i);
            saveToSO.positions.Add(child.localPosition);
            saveToSO.rotations.Add(child.localRotation);

            GameObject prefab = PrefabUtility.GetCorrespondingObjectFromSource(child.gameObject);
            if (prefab != null)
            {
                saveToSO.prefabs.Add(prefab);
            }
            else
            {
                Debug.LogError($"Could not find prefab source for {child.name}. Make sure it’s a prefab instance!");
                saveToSO.prefabs.Add(null); // keep alignment
            }
        }

        UnityEditor.EditorUtility.SetDirty(saveToSO);
        UnityEditor.AssetDatabase.SaveAssets();

        Debug.Log($"Exported {saveToSO.positions.Count} children (pos+rot+prefab) to ScriptableObject.");
#endif
    }


    [ContextMenu("Blind Restore From ScriptableObject")]
    public void BlindRestoreFromSO()
    {
        if (saveToSO == null ||
            saveToSO.positions == null ||
            saveToSO.prefabs == null ||
            saveToSO.positions.Count == 0)
        {
            Debug.LogError("ScriptableObject data is missing or incomplete.");
            return;
        }

        // Clear old instances
        for (int i = transform.childCount - 1; i >= 0; i--)
            DestroyImmediate(transform.GetChild(i).gameObject);
        instances.Clear();

        for (int i = 0; i < saveToSO.positions.Count; i++)
        {
            GameObject prefab = saveToSO.prefabs[i];
            if (prefab == null)
            {
                Debug.LogWarning($"⚠️ Missing prefab at index {i}, skipping.");
                continue;
            }

            Vector3 localPos = saveToSO.positions[i];
            Quaternion localRot = (saveToSO.rotations != null && i < saveToSO.rotations.Count)
                ? saveToSO.rotations[i]
                : Quaternion.identity;

            GameObject temp = null;

#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                temp = (GameObject)PrefabUtility.InstantiatePrefab(prefab, transform);
                temp.transform.localPosition = localPos;
                temp.transform.localRotation = localRot;
            }
            else
            {
                temp = Instantiate(prefab, transform);
                temp.transform.localPosition = localPos;
                temp.transform.localRotation = localRot;
            }
#else
            temp = Instantiate(prefab, transform);
            temp.transform.localPosition = localPos;
            temp.transform.localRotation = localRot;
#endif

                instances.Add(temp.transform);
        }

        Debug.Log($" Blind restored {instances.Count} objects from ScriptableObject.");
    }

#endif

    private void Awake()
    {
        navMeshSurfaces = GetComponents<NavMeshSurface>();
    }

    private void Start()
    {
        if (!Application.isPlaying) return;

        if (difficultyLevel >= formations.Count)
        {
            Debug.LogWarning($"{gameObject.name} has no config for this difficulty level, using last available.");
            difficultyLevel = formations.Count - 1;
        }

        ChallengeManager.instance.OnChallengeFail += DeleteSpawnedGrid;
        //SetupFormation();
    }
    private void OnDisable()
    {
        if (ChallengeManager.instance != null)
        {
            ChallengeManager.instance.OnChallengeFail -= DeleteSpawnedGrid;
        }
    }

    private IEnumerator SpawnFormation(List<Vector3> positions)
    {
        yield return StartCoroutine(SpawnFormationGradually(positions));
        //yield return StartCoroutine(BuildNavMeshGradually());
    }

    public void DeleteSpawnedGrid()
    {
        StartCoroutine(DeleteRoutine());
    }

    IEnumerator DeleteRoutine()
    {
        transform.parent = null;
        yield return new WaitForSeconds(5f); 
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            Destroy(transform.GetChild(i).gameObject);
            yield return null;
        }
        instances.Clear();
        positionsPerFormation.Clear();
    }
    //void Update()
    //{
    //    if (!Application.isPlaying || instances.Count == 0 || formations.Count < 2) return;
    //    if (!isTransitioning && Input.GetKeyDown(KeyCode.G))
    //    {
    //        NextTransition();
    //    }
    //    if (isTransitioning)
    //    {
    //        timer += Time.deltaTime;
    //        float t = Mathf.Clamp01(timer / transitionDuration);
    //        for (int i = 0; i < instances.Count; i++)
    //        {
    //            Vector3 currentPos = transitionStart[i];
    //            Vector3 targetPos = transitionTarget[i];
    //            instances[i].localPosition = new Vector3(
    //                currentPos.x,
    //                Mathf.Lerp(currentPos.y, targetPos.y, t),
    //                currentPos.z
    //            );

    //        }
    //        //instances[i].localPosition = Vector3.Lerp(transitionStart[i], transitionTarget[i], t); // Update X and Z also
    //        if (timer >= transitionDuration)
    //        {
    //            isTransitioning = false;
    //            currentIndex = (currentIndex + 1) % formations[difficultyLevel].config.Count;
    //        }
    //    }
    //}

    [ContextMenu("Setup Formation from ScriptableObject")]
    public IEnumerator SetupFormation()
    {
        if (gridDatabase == null)
        {
            Debug.LogError("Grid database is not assigned.");
            yield break;
        }

        difficultyLevel = DungeonManager.Instance.DifficultyLevel - 1;
        groupKey = ChallengeManager.instance.CurrentChallenge.GetID();

        GridFormationData data = saveToSO;

        if (data == null || data.positions == null || data.positions.Count == 0)
        {
            Debug.LogError("Grid formation data is missing or empty.");
            yield break;
        }

        StartCoroutine(BlindRestoreFromSOGradual());
    }

    [ContextMenu("Blind Restore From SO (Gradual)")]
    public void StartBlindRoutine() => StartCoroutine(BlindRestoreFromSOGradual());
    public IEnumerator BlindRestoreFromSOGradual()
    {
        if (saveToSO == null ||
            saveToSO.positions == null ||
            saveToSO.prefabs == null ||
            saveToSO.positions.Count == 0)
        {
            Debug.LogError(" ScriptableObject data is missing or incomplete.");
            yield break;
        }

        // Clear old instances
        for (int i = transform.childCount - 1; i >= 0; i--)
            DestroyImmediate(transform.GetChild(i).gameObject);
        instances.Clear();

        int count = 0;

        for (int i = 0; i < saveToSO.positions.Count; i++)
        {
            GameObject prefab = saveToSO.prefabs[i];
            if (prefab == null)
            {
                Debug.LogWarning($"Missing prefab at index {i}, skipping.");
                continue;
            }

            Vector3 localPos = saveToSO.positions[i];
            Quaternion localRot = (saveToSO.rotations != null && i < saveToSO.rotations.Count)
                ? saveToSO.rotations[i]
                : Quaternion.identity;


            GameObject temp = null;

#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                temp = (GameObject)PrefabUtility.InstantiatePrefab(prefab, transform);
                temp.transform.localPosition = localPos;
                temp.transform.localRotation = localRot;
            }
            else
            {
                temp = Instantiate(prefab, transform);
                temp.transform.localPosition = localPos;
                temp.transform.localRotation = localRot;
            }
#else
            temp = Instantiate(prefab, transform);
            temp.transform.localPosition = localPos;
            temp.transform.localRotation = localRot;
#endif


            instances.Add(temp.transform);
            count++;

            temp.transform.GetChild(0).gameObject.SetActive(false);

            // Yield every N spawns
            if (count % spawnPerFrame == 0)
                yield return null;
        }

        // Final yield if we ended mid-frame
        if (count % spawnPerFrame != 0)
            yield return null;

        Debug.Log($"Blind restored {instances.Count} objects gradually from ScriptableObject.");
    }

#if UNITY_EDITOR
    [ContextMenu("Load Current Formation")]
    public void LoadRandomFormationFromDatabase()
    {
        var data = saveToSO;
        if (data == null || data.positions == null || data.positions.Count == 0)
        {
            Debug.LogError("No grid data returned.");
            return;
        }

        if (instances.Count != 0)
        {
            Editor_DeleteSpawnedGrid();
        }

        currentIndex = 0;

        isExactRestore = true;
        RecomputeFormationsFromBase(data.positions, data.rotations);
        StartCoroutine(SpawnFormation(0));
        isExactRestore = false;
    }
#endif
    [ContextMenu("Reload Random Formation")]
    public void ReloadRandomFormation(string ID)
    {
        // Clean up existing instances
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(transform.GetChild(i).gameObject);
        }
        instances.Clear();
        positionsPerFormation.Clear();

        // Get new formation from DB
        var data = gridDatabase.GetRandomUniqueFormation(ID);
        if (data == null || data.positions == null || data.positions.Count == 0)
        {
            Debug.LogError("Failed to reload formation: no valid data.");
            return;
        }

        // Recompute positions and spawn
        RecomputeFormationsFromBase(data.positions);
        currentIndex = 0;

        StartCoroutine(SpawnFormation(positionsPerFormation[currentIndex]));
        Debug.Log("Reloaded new formation from group: " + groupKey);
    }

    private void RecomputeFormationsFromBase(List<Vector3> basePositions, List<Quaternion> baseRotations = null)
    {
        if (isExactRestore)
        {
            positionsPerFormation.Clear();
            // treat them as local, not world
            positionsPerFormation.Add(new List<Vector3>(basePositions));
            savedRotations = baseRotations != null ? new List<Quaternion>(baseRotations) : null;
            return;
        }


        gridSpanX = basePositions.Max(p => p.x) - basePositions.Min(p => p.x);
        gridSpanZ = basePositions.Max(p => p.z) - basePositions.Min(p => p.z);
        gridCenter = new Vector3(
            (basePositions.Min(p => p.x) + basePositions.Max(p => p.x)) / 2f,
            0,
            (basePositions.Min(p => p.z) + basePositions.Max(p => p.z)) / 2f
        );

        positionsPerFormation.Clear();

        for (int f = 0; f < formations[difficultyLevel].config.Count; f++)
        {
            var config = formations[difficultyLevel].config[f];
            float safeY = maxJitterY * (1f - density);

            var list = new List<Vector3>();
            foreach (var p in basePositions)
            {
                float y = EvaluateFormation(config.type, new Vector3(p.x, 0, p.z), f);
                if (config.jitterY && !isExactRestore) // Y jitter only in procedural mode
                    y += Random.Range(-safeY, safeY);

                list.Add(new Vector3(p.x, y, p.z));
            }

            // Skip radius clearing when restoring exact save
            if (!isExactRestore)
            {
                foreach (var rp in radiusPrefabs)
                {
                    if (rp.centerElement == null || rp.centralElementRadius <= 0f) continue;

                    Vector3 centerXZ = gridCenter + rp.centerOffset;
                    float y = EvaluateFormation(config.type, centerXZ, f);
                    Vector3 centerPos = new Vector3(centerXZ.x, y, centerXZ.z);

                    // 1. Remove everything inside the radius
                    list.RemoveAll(p =>
                    {
                        // if it's basically the exact center, keep it
                        if (Vector3.Distance(p, centerPos) < 0.01f)
                            return false;
                        // otherwise, delete if inside central radius
                        return Vector3.Distance(new Vector2(p.x, p.z), new Vector2(centerXZ.x, centerXZ.z)) <= rp.centralElementRadius;
                    });

                    // 2. Add the central prefab if missing
                    if (!list.Any(p => Vector3.Distance(p, centerPos) < 0.01f))
                        list.Add(centerPos);
                }

            }

            foreach (var rp in radiusPrefabs)
            {
                if (rp.centerElement != null)
                {
                    Vector3 centerXZ = gridCenter + rp.centerOffset;
                    float y = EvaluateFormation(config.type, centerXZ, f);
                    Vector3 centerPos = new Vector3(centerXZ.x, y, centerXZ.z);

                    bool alreadyExists = list.Any(p => Vector3.Distance(p, centerPos) < 0.01f);
                    if (!alreadyExists)
                        list.Add(centerPos);
                }
            }


            positionsPerFormation.Add(list);
        }

        if (isExactRestore && baseRotations != null && baseRotations.Count == basePositions.Count)
        {
            savedRotations = new List<Quaternion>(baseRotations);
        }
        else
        {
            savedRotations = null;
        }
    }


    public override void NextTransition()
    {
        isTransitioning = true;
        timer = 0f;
        transitionStart = new List<Vector3>(GetCurrentPositions());
        int next = (currentIndex + 1) % formations[difficultyLevel].config.Count;
        transitionTarget = new List<Vector3>(positionsPerFormation[next]);
    }


    void InitializeFormations()
    {
        var sample = cuboidPrefabs[0].prefab;

        //── build base positions with random spacing ──
        List<Vector3> basePositionsList = new List<Vector3>();
        List<Vector2> jitterXZList = new List<Vector2>();

        for (int x = 0; x < gridWidth; x++)
        {
            float spX = Random.Range(minSpacing, maxSpacing);
            for (int z = 0; z < gridDepth; z++)
            {
                float spZ = Random.Range(minSpacing, maxSpacing);
                if (Random.value > density) continue;

                float rawX = transform.position.x
                           + x * (sample.GetComponent<Renderer>().bounds.size.x + spX);
                float rawZ = transform.position.z
                           + z * (sample.GetComponent<Renderer>().bounds.size.z + spZ);

                basePositionsList.Add(new Vector3(rawX, transform.position.y, rawZ));

                float maxJ = Mathf.Min(maxJitterXZ, minSpacing * 0.5f) * (1f - density);
                float jx = Random.Range(-maxJ, maxJ);
                float jz = Random.Range(-maxJ, maxJ);
                jitterXZList.Add(new Vector2(rawX + jx, rawZ + jz));
            }
        }

        //── recompute span from grid + radius centers ──
        List<Vector3> spanCandidates = new List<Vector3>(basePositionsList);
        foreach (var rp in radiusPrefabs)
        {
            if (rp.centerElement != null)
            {
                Vector3 centerXZ = transform.position + rp.centerOffset;
                spanCandidates.Add(centerXZ);
            }
        }

        float minX = transform.position.x;
        float minZ = transform.position.z;
        float maxX = basePositionsList.Max(p => p.x);
        float maxZ = basePositionsList.Max(p => p.z);

        gridSpanX = maxX - minX;
        gridSpanZ = maxZ - minZ;
        gridCenter = transform.position + new Vector3(gridSpanX / 2f, 0, gridSpanZ / 2f);


        //── build per-formation variations ──
        positionsPerFormation.Clear();
        for (int f = 0; f < formations[difficultyLevel].config.Count; f++)
        {
            var config = formations[difficultyLevel].config[f];
            float safeY = maxJitterY * (1f - density);

            var list = new List<Vector3>();
            for (int i = 0; i < basePositionsList.Count; i++)
            {
                float px = jitterXZList[i].x;
                float pz = jitterXZList[i].y;

                float y = EvaluateFormation(config.type, new Vector3(px, transform.position.y, pz), f);
                if (config.jitterY)
                    y += Random.Range(-safeY, safeY);

                list.Add(new Vector3(px, y, pz));
            }

            //── remove any grid positions inside radius (but never the center) ──
            if (!isExactRestore)
            {
                foreach (var rp in radiusPrefabs)
                {
                    if (rp.centerElement == null || rp.centralElementRadius <= 0f) continue;

                    Vector3 centerXZ = gridCenter + rp.centerOffset;
                    float y = EvaluateFormation(config.type, centerXZ, f);
                    Vector3 centerPos = new Vector3(centerXZ.x, y, centerXZ.z);

                    // 1. Remove everything inside the radius
                    list.RemoveAll(p =>
                    {
                        // if it's basically the exact center, keep it
                        if (Vector3.Distance(p, centerPos) < 0.01f)
                            return false;
                        // otherwise, delete if inside central radius
                        return Vector3.Distance(new Vector2(p.x, p.z), new Vector2(centerXZ.x, centerXZ.z)) <= rp.centralElementRadius;
                    });

                    // 2. Add the central prefab if missing
                    if (!list.Any(p => Vector3.Distance(p, centerPos) < 0.01f))
                        list.Add(centerPos);
                }

            }


            //── add back exact radius centers ──
            foreach (var rp in radiusPrefabs)
            {
                if (rp.centerElement != null)
                {
                    Vector3 centerXZ = gridCenter + rp.centerOffset;
                    float y = EvaluateFormation(config.type, centerXZ, f);
                    Vector3 centerPos = new Vector3(centerXZ.x, y, centerXZ.z);

                    bool alreadyExists = list.Any(p => Vector3.Distance(p, centerPos) < 0.01f);
                    if (!alreadyExists)
                        list.Add(centerPos);
                }
            }


            positionsPerFormation.Add(list);
        }
    }

    private IEnumerator SpawnFormationGradually(List<Vector3> positions)
    {
        int count = 0;

        for (int i = 0; i < positions.Count; i++)
        {
            Vector3 pos = positions[i];
            GameObject prefabToSpawn = null;
            bool isRingCenter = false;

            if (isExactRestore && saveToSO.prefabs != null && i < saveToSO.prefabs.Count)
            {
                prefabToSpawn = saveToSO.prefabs[i];
            }

            // --- Center element detection ---
            if (prefabToSpawn == null)
            {
                Vector2 posXZ = new Vector2(pos.x, pos.z);
                foreach (var rp in radiusPrefabs)
                {
                    if (rp.centerElement == null) continue;

                    Vector2 ringCenterXZ = new Vector2(
                        gridCenter.x + rp.centerOffset.x,
                        gridCenter.z + rp.centerOffset.z
                    );

                    if (Vector2.Distance(posXZ, ringCenterXZ) < 0.001f)
                    {
                        prefabToSpawn = rp.centerElement;
                        isRingCenter = true;
                        break;
                    }
                }
            }

            // --- Fallback to weighted selection ---
            if (prefabToSpawn == null)
                prefabToSpawn = GetWeightedRandomPrefabAtPosition(pos);

            Vector3 worldPos = transform.TransformPoint(pos);

            Quaternion rotationToUse;
            if (isExactRestore && savedRotations != null && i < savedRotations.Count)
            {
                rotationToUse = transform.rotation * savedRotations[i];
            }
            else
            {
                float randomY = 90f * Random.Range(0, 4);
                rotationToUse = transform.rotation * Quaternion.Euler(0f, randomY, 0f);
            }

            GameObject temp = null;
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                temp = (GameObject)PrefabUtility.InstantiatePrefab(prefabToSpawn, transform);
                temp.transform.localPosition = pos;
                temp.transform.localRotation = rotationToUse;
            }
            else
#endif
            {
                temp = Instantiate(prefabToSpawn, worldPos, rotationToUse, transform);
            }

            // Skip collision filtering when restoring exact save
            if (!isExactRestore && !isRingCenter)
            {
                var climb = temp.GetComponentInChildren<ClimbInteractable>();
                if (climb != null)
                {
                    Collider[] colliders = climb.GetComponentsInChildren<Collider>();
                    bool collided = false;

                    foreach (var col in colliders)
                    {
                        Collider[] hits = Physics.OverlapBox(
                            col.bounds.center,
                            col.bounds.extents * 0.95f,
                            col.transform.rotation,
                            ~0,
                            QueryTriggerInteraction.Ignore
                        );

                        foreach (var hit in hits)
                        {
                            if (hit.transform.IsChildOf(temp.transform)) continue;
                            if (hit.GetComponentInParent<ClimbInteractable>() != null)
                            {
                                collided = true;
                                break;
                            }
                        }

                        if (collided) break;
                    }

                    if (collided)
                    {
                        DestroyImmediate(temp);
                        continue;
                    }
                }
            }

            instances.Add(temp.transform);
            count++;

            if (count % spawnPerFrame == 0)
                yield return null;
        }

        if (count % spawnPerFrame != 0)
            yield return null;
    }


    IEnumerator SpawnFormation(int idx)
    {
        // Clear existing
        for (int i = transform.childCount - 1; i >= 0; i--)
            DestroyImmediate(transform.GetChild(i).gameObject);
        instances.Clear();

        var positions = positionsPerFormation[idx];

        for (int i = 0; i < positions.Count; i++)
        {
            Vector3 pos = positions[i];
            GameObject prefabToSpawn = null;
            bool isRingCenter = false;

            if (isExactRestore && saveToSO.prefabs != null && i < saveToSO.prefabs.Count)
            {
                prefabToSpawn = saveToSO.prefabs[i];
            }

            // --- Center element detection ---
            if (prefabToSpawn == null)
            {
                Vector2 posXZ = new Vector2(pos.x, pos.z);
                foreach (var rp in radiusPrefabs)
                {
                    if (rp.centerElement == null) continue;

                    Vector2 ringCenterXZ = new Vector2(
                        gridCenter.x + rp.centerOffset.x,
                        gridCenter.z + rp.centerOffset.z
                    );

                    if (Vector2.Distance(posXZ, ringCenterXZ) < 0.001f)
                    {
                        prefabToSpawn = rp.centerElement;
                        isRingCenter = true;
                        break;
                    }
                }
            }

            // --- Fallback to weighted selection ---
            if (prefabToSpawn == null)
                prefabToSpawn = GetWeightedRandomPrefabAtPosition(pos);

            Vector3 worldPos = transform.TransformPoint(pos);

            Quaternion rotationToUse;
            if (isExactRestore && savedRotations != null && i < savedRotations.Count)
            {
                rotationToUse = transform.rotation * savedRotations[i];
            }
            else
            {
                float randomY = 90f * Random.Range(0, 4);
                rotationToUse = transform.rotation * Quaternion.Euler(0f, randomY, 0f);
            }

            GameObject temp = null;
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                if (prefabToSpawn == null)
                {
                    Debug.LogWarning($"Prefab missing at index {i}");
                    continue;
                }

                temp = (GameObject)PrefabUtility.InstantiatePrefab(prefabToSpawn, transform);
                temp.transform.localPosition = pos;          // safer than SetPositionAndRotation
                temp.transform.localRotation = Quaternion.identity;
                temp.transform.localScale = prefabToSpawn.transform.localScale;
                UnityEditor.EditorUtility.SetDirty(temp);
                yield return null; // wait a frame for prefab to initialize properly
            }
            else
#endif
            {
                temp = Instantiate(prefabToSpawn, worldPos, rotationToUse, transform);
            }


            // Skip collision checks when restoring exact save
            if (!isExactRestore && !isRingCenter)
            {
                var climb = temp.GetComponentInChildren<ClimbInteractable>();
                if (climb != null)
                {
                    Collider[] colliders = climb.GetComponentsInChildren<Collider>();
                    bool collided = false;

                    foreach (var col in colliders)
                    {
                        Collider[] hits = Physics.OverlapBox(
                            col.bounds.center,
                            col.bounds.extents * 0.95f,
                            col.transform.rotation,
                            ~0,
                            QueryTriggerInteraction.Ignore
                        );

                        foreach (var hit in hits)
                        {
                            if (hit.transform.IsChildOf(temp.transform)) continue;
                            if (hit.GetComponentInParent<ClimbInteractable>() != null)
                            {
                                collided = true;
                                break;
                            }
                        }

                        if (collided) break;
                    }

                    if (collided)
                    {
                        DestroyImmediate(temp);
                        continue;
                    }
                }
            }

            instances.Add(temp.transform);
        }
        yield return null;
    }

    List<Vector3> GetCurrentPositions()
    {
        var list = new List<Vector3>();
        instances.ForEach(t => list.Add(t.localPosition));
        return list;
    }

    float EvaluateFormation(FormationTypeGround type, Vector3 pos, int idx)
    {
        switch (type)
        {
            case FormationTypeGround.Crater: return EvaluateCrater(pos);
            case FormationTypeGround.Sine: return EvaluateSine(pos);
            case FormationTypeGround.CustomWave: return EvaluateCustomWave(pos, idx);
            default: return 0f;
        }
    }

    float EvaluateCrater(Vector3 w)
    {
        float d = Vector2.Distance(new Vector2(w.x, w.z), new Vector2(gridCenter.x, gridCenter.z));
        if (d <= innerRadius) return -maxDepth; if (d >= outerRadius) return 0f; float t = (d - innerRadius) / (outerRadius - innerRadius); return -maxDepth * (1f - t);
    }
    float EvaluateSine(Vector3 w) { float nx = (w.x - transform.position.x) / gridSpanX; return Mathf.Sin(nx * Mathf.PI * sineFrequency) * sineAmplitude; }
    float EvaluateCustomWave(Vector3 w, int idx)
    {
        int occ = 0; for (int i = 0; i <= idx; i++) if (formations[difficultyLevel].config[i].type == FormationTypeGround.CustomWave) occ++; int iC = Mathf.Clamp(occ - 1, 0, customWaveSettings.Count - 1);
        var s = customWaveSettings[iC]; float y = 0f;
        if (s.axis == WaveAxis.X || s.axis == WaveAxis.Both) { float tx = (w.x - transform.position.x) / gridSpanX; y += s.curve.Evaluate(tx); }
        if (s.axis == WaveAxis.Z || s.axis == WaveAxis.Both) { float tz = (w.z - transform.position.z) / gridSpanZ; y += s.curve.Evaluate(tz); }
        if (s.axis == WaveAxis.Both) y *= 0.5f; return y * s.amplitude;
    }

    GameObject GetWeightedRandomPrefabAtPosition(Vector3 pos)
    {
        //If you want a specific priority—e.g. always let
        // smaller radii override bigger ones—sort ascending by radius:
        radiusPrefabs.Sort((a, b) => a.radius.CompareTo(b.radius));
        // Or, if you want the opposite priority, sort descending by radius instead.

        for (int i = 0; i < radiusPrefabs.Count; i++)
        {
            var rp = radiusPrefabs[i];
            // Compute this ring’s true center in world‐space:
            Vector3 ringCenter = gridCenter + rp.centerOffset;

            // Distance from pos to this ring’s center (XZ‐plane):
            float distance = Vector2.Distance(
                new Vector2(pos.x, pos.z),
                new Vector2(ringCenter.x, ringCenter.z)
            );

            // If pos lies inside this ring’s radius, pick from its prefab formations:
            if (distance <= rp.radius)
            {
                return GetWeightedFromList(rp.prefabs);
            }
        }

        // If no ring matched, fall back to corner:
        return GetWeightedFromList(cornerPrefabs);
    }


    GameObject GetWeightedFromList(List<WeightedPrefab> list)
    {
        if (list == null || list.Count == 0) return cuboidPrefabs[0].prefab;

        float totalWeight = 0f;
        foreach (var wp in list)
            totalWeight += wp.weight;

        float r = Random.value * totalWeight;
        float acc = 0f;
        foreach (var wp in list)
        {
            acc += wp.weight;
            if (r <= acc)
                return wp.prefab;
        }

        return list[0].prefab;
    }

    float EstimateTotalSpan(GameObject s, int c, float minS, float maxS) { float size = s.GetComponent<Renderer>().bounds.size.x; float avg = (minS + maxS) * 0.5f; return c * size + (c - 1) * avg; }

    public Vector3 GetGridCentre() => gridCenter;
    public float GetGridRadius()
    {
        float maxRadius = 0f;
        for (int i = 0; i < radiusPrefabs.Count; i++)
        {
            maxRadius = Mathf.Max(maxRadius, radiusPrefabs[i].radius);
        }

        return maxRadius;
    }

    public void OnDestroy()
    {
        // Clean up instances
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            Destroy(transform.GetChild(i).gameObject);
        }
        instances.Clear();
        positionsPerFormation.Clear();
    }

#if UNITY_EDITOR
#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        if (cuboidPrefabs == null || cuboidPrefabs.Count == 0) return;
        if (drawGizmos == false) return;

        // Ensure formations are initialized for gizmos
        if (positionsPerFormation == null || positionsPerFormation.Count != formations.Count)
        {
            InitializeFormations();
        }

        // Draw spawn positions
        int idx = Mathf.Clamp(currentIndex, 0, positionsPerFormation.Count - 1);
        var list = positionsPerFormation[idx];
        Gizmos.color = Color.cyan;
        foreach (var localPos in list)
        {
            Vector3 worldPos = transform.TransformPoint(localPos);
            Gizmos.DrawWireCube(worldPos, Vector3.one * 0.1f);
        }

        // Draw grid outline
        Gizmos.color = Color.yellow;
        Vector3 center = transform.TransformPoint(gridCenter);
        float width = gridSpanX;
        float depth = gridSpanZ;

        Vector3 right = transform.right * width;
        Vector3 forward = transform.forward * depth;

        Vector3 bottomLeft = center - right / 2f - forward / 2f;
        Vector3 bottomRight = bottomLeft + right;
        Vector3 topLeft = bottomLeft + forward;
        Vector3 topRight = bottomLeft + right + forward;

        Gizmos.DrawLine(bottomLeft, bottomRight);
        Gizmos.DrawLine(bottomRight, topRight);
        Gizmos.DrawLine(topRight, topLeft);
        Gizmos.DrawLine(topLeft, bottomLeft);

        // Draw circles for radius prefabs
        if (radiusPrefabs != null && radiusPrefabs.Count > 0)
        {
            Gizmos.color = Color.green;
            foreach (var rp in radiusPrefabs)
            {
                float radius = rp.radius;
                Vector3 customCenter = transform.TransformPoint(gridCenter + rp.centerOffset);

                int segments = 64;
                float angleStep = 360f / segments;
                Vector3 prevPoint = customCenter + (transform.right * Mathf.Cos(0) + transform.forward * Mathf.Sin(0)) * radius;

                for (int i = 1; i <= segments; i++)
                {
                    float rad = Mathf.Deg2Rad * (i * angleStep);
                    Vector3 nextPoint = customCenter + (transform.right * Mathf.Cos(rad) + transform.forward * Mathf.Sin(rad)) * radius;
                    Gizmos.DrawLine(prevPoint, nextPoint);
                    prevPoint = nextPoint;
                }
            }
        }
    }

#endif

#endif
}

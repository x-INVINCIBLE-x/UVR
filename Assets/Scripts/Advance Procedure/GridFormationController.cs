using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;

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

    private float gridSpanX, gridSpanZ; 
    private Vector3 gridCenter;
    private int currentIndex = 0; 
    private bool isTransitioning = false; 
    private float timer = 0f;
    private int difficultyLevel = 1;
    private NavMeshSurface[] navMeshSurfaces;

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

        InitializeFormations();

        if (positionsPerFormation.Count == 0 || positionsPerFormation[0].Count == 0)
        {
            Debug.LogError("Failed to initialize base formation.");
            return;
        }

        SpawnFormation(0); // Use the 0 index for spawn formation
        Debug.Log("Initial formation spawned. You may now reposition the blocks.");
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

        Debug.Log("Saved current positions as base spawn formation at index 0.");
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

    [ContextMenu("Editor: Export Formation[0] to ScriptableObject")]
    private void Editor_ExportToScriptableObject()
    {
        if (saveToSO == null)
        {
            Debug.LogError("No ScriptableObject assigned to saveToSO.");
            return;
        }

        if (positionsPerFormation.Count == 0 || positionsPerFormation[0].Count == 0)
        {
            Debug.LogError("No formation data to save.");
            return;
        }

        saveToSO.positions = new List<Vector3>(positionsPerFormation[0]);

        UnityEditor.EditorUtility.SetDirty(saveToSO);
        UnityEditor.AssetDatabase.SaveAssets();

        Debug.Log("Exported Formation[0] to ScriptableObject.");
    }

#endif

    private void Awake()
    {
        navMeshSurfaces = GetComponents<NavMeshSurface>();
    }

    private void Start()
    {
        if (!Application.isPlaying) return;

        difficultyLevel = DungeonManager.Instance.DifficultyLevel - 1;

        if (difficultyLevel >= formations.Count)
        {
            Debug.LogWarning($"{gameObject.name} has no config for this difficulty level, using last available.");
            difficultyLevel = formations.Count - 1;
        }
        SetupFormationFromDatabaseAtRuntime();
    }


    private IEnumerator SpawnFormationGraduallyThenBuildNavMesh(List<Vector3> positions)
    {
        yield return StartCoroutine(SpawnFormationGradually(positions));
        //yield return StartCoroutine(BuildNavMeshGradually());
        if (PrioritySceneGate.Instance != null)
        {
            PrioritySceneGate.Instance.MarkReady();
            yield return new WaitForEndOfFrame();
            yield return new WaitForSeconds(0.1f);
            PrioritySceneGate.Instance.MarkUnready();
        }
    }

    void Update()
    {
        if (!Application.isPlaying || instances.Count == 0 || formations.Count < 2) return;
        if (!isTransitioning && Input.GetKeyDown(KeyCode.G))
        {
            NextTransition();
        }
        if (isTransitioning)
        {
            timer += Time.deltaTime;
            float t = Mathf.Clamp01(timer / transitionDuration);
            for (int i = 0; i < instances.Count; i++)
            {
                Vector3 currentPos = transitionStart[i];
                Vector3 targetPos = transitionTarget[i];
                instances[i].localPosition = new Vector3(
                    currentPos.x,
                    Mathf.Lerp(currentPos.y, targetPos.y, t),
                    currentPos.z
                );

            }
            //instances[i].localPosition = Vector3.Lerp(transitionStart[i], transitionTarget[i], t); // Update X and Z also
            if (timer >= transitionDuration)
            {
                isTransitioning = false;
                currentIndex = (currentIndex + 1) % formations[difficultyLevel].config.Count;
            }
        }
    }

    private void SetupFormationFromDatabaseAtRuntime()
    {
        if (gridDatabase == null)
        {
            Debug.LogError("Grid database is not assigned.");
            return;
        }

        groupKey = ChallengeManager.instance.CurrentChallenge.GetID();
        GridFormationData data = gridDatabase.GetRandomUniqueFormation(groupKey);

        if (data == null || data.positions == null || data.positions.Count == 0)
        {
            Debug.LogError("Grid formation data is missing or empty.");
            return;
        }

        // Use this base to compute all formations
        RecomputeFormationsFromBase(data.positions);
        currentIndex = 0;

        // Gradual spawn with navmesh bake
        StartCoroutine(SpawnFormationGraduallyThenBuildNavMesh(positionsPerFormation[currentIndex]));
    }

    [ContextMenu("Load Random Formation From Database")]
    public void LoadRandomFormationFromDatabase()
    {
        if (gridDatabase == null)
        {
            Debug.LogError("Grid database not assigned.");
            return;
        }

        var data = gridDatabase.GetRandomUniqueFormation(groupKey);
        if (data == null || data.positions == null || data.positions.Count == 0)
        {
            Debug.LogError("No grid data returned.");
            return;
        }

        // Convert to basePositionsList and recompute all formations
        RecomputeFormationsFromBase(data.positions);

        currentIndex = 0;

        if (instances.Count == 0)
        {
            SpawnFormation(0);
        }
        else
        {
            RepositionInstances(positionsPerFormation[0]);
        }
    }

    private void RepositionInstances(List<Vector3> positions)
    {
        if (instances.Count != positions.Count)
        {
            Debug.LogWarning("Instance count mismatch. Destroying and respawning.");
            SpawnFormation(0);
            return;
        }

        for (int i = 0; i < instances.Count; i++)
        {
            instances[i].localPosition = positions[i];
        }
    }

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

        StartCoroutine(SpawnFormationGraduallyThenBuildNavMesh(positionsPerFormation[currentIndex]));
        Debug.Log("Reloaded new formation from group: " + groupKey);
    }

    private void RecomputeFormationsFromBase(List<Vector3> basePositions)
    {
        // Update grid span based on loaded data
        if (cuboidPrefabs.Count > 0)
        {
            var sample = cuboidPrefabs[0].prefab;
            gridSpanX = EstimateTotalSpan(sample, gridWidth, minSpacing, maxSpacing);
            gridSpanZ = EstimateTotalSpan(sample, gridDepth, minSpacing, maxSpacing);
        }
        gridCenter = transform.position + new Vector3(gridSpanX / 2f, 0, gridSpanZ / 2f);

        positionsPerFormation.Clear();

        var baseXZ = new List<Vector2>();
        foreach (var p in basePositions)
            baseXZ.Add(new Vector2(p.x, p.z));

        for (int f = 0; f < formations[difficultyLevel].config.Count; f++)
        {
            var config = formations[difficultyLevel].config[f];
            float safeY = maxJitterY * (1f - density);
            var list = new List<Vector3>();

            for (int i = 0; i < baseXZ.Count; i++)
            {
                float px = baseXZ[i].x;
                float pz = baseXZ[i].y;
                float y = EvaluateFormation(config.type, new Vector3(px, transform.position.y, pz), f);
                if (config.jitterY)
                    y += Random.Range(-safeY, safeY);
                list.Add(new Vector3(px, y, pz));
            }

            // Clear central radius areas
            foreach (var rp in radiusPrefabs)
            {
                if (rp.centerElement != null && rp.centralElementRadius > 0f)
                {
                    Vector2 cXZ = new Vector2(
                        gridCenter.x + rp.centerOffset.x,
                        gridCenter.z + rp.centerOffset.z
                    );

                    list.RemoveAll(p =>
                        Vector2.Distance(new Vector2(p.x, p.z), cXZ)
                        <= rp.centralElementRadius
                    );
                }
            }

            // Add ring centers last
            foreach (var rp in radiusPrefabs)
            {
                if (rp.centerElement != null)
                {
                    Vector3 centerXZ = gridCenter + rp.centerOffset;
                    float y = EvaluateFormation(config.type, centerXZ, f);
                    list.Add(new Vector3(centerXZ.x, y, centerXZ.z));
                }
            }

            positionsPerFormation.Add(list);
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
        //── compute spans (unchanged) ──
        var sample = cuboidPrefabs[0].prefab;
        gridSpanX = EstimateTotalSpan(sample, gridWidth, minSpacing, maxSpacing);
        gridSpanZ = EstimateTotalSpan(sample, gridDepth, minSpacing, maxSpacing);
        gridCenter = transform.position + new Vector3(gridSpanX / 2f, 0, gridSpanZ / 2f);

        //── build a consistent formations of “base” XZ positions with one‐time jitter ──
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

        positionsPerFormation.Clear();

        for (int f = 0; f < formations[difficultyLevel].config.Count; f++)
        {
            var config = formations[difficultyLevel].config[f];
            float safeY = maxJitterY * (1f - density);

            // 1) Build all grid positions (with frozen XZ jitter, variable Y)
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

            // 2) Now remove any grid position inside a ring’s clear radius
            foreach (var rp in radiusPrefabs)
            {
                if (rp.centerElement != null && rp.centralElementRadius > 0f)
                {
                    Vector2 cXZ = new Vector2(
                        gridCenter.x + rp.centerOffset.x,
                        gridCenter.z + rp.centerOffset.z
                    );

                    list.RemoveAll(p =>
                        Vector2.Distance(new Vector2(p.x, p.z), cXZ)
                        <= rp.centralElementRadius
                    );
                }
            }

            // 3) After clearing nearby grid points, append each center’s exact position last
            foreach (var rp in radiusPrefabs)
            {
                if (rp.centerElement != null)
                {
                    Vector3 centerXZ = gridCenter + rp.centerOffset;
                    float y = EvaluateFormation(formations[difficultyLevel].config[f].type, centerXZ, f);
                    // (No Y‐jitter here—keeps matching simple.)
                    list.Add(new Vector3(centerXZ.x, y, centerXZ.z));
                }
            }

            positionsPerFormation.Add(list);
        }
    }


    private IEnumerator SpawnFormationGradually(List<Vector3> positions)
    {
        int count = 0;

        foreach (var pos in positions)
        {
            GameObject prefabToSpawn = null;

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
                    break;
                }
            }

            if (prefabToSpawn == null)
                prefabToSpawn = GetWeightedRandomPrefabAtPosition(pos);

            Quaternion randomYRotation = Quaternion.Euler(0f, 90f * Random.Range(0, 4), 0f);
            GameObject newBlock = Instantiate(prefabToSpawn, pos + new Vector3(0, transform.position.y, 0), randomYRotation, transform);
            try
            {
                if (deactivateOnSpawn)
                    newBlock.transform.GetChild(0).gameObject.SetActive(false);

            }
            catch
            {
                Debug.LogWarning("Prefab " + prefabToSpawn.name + " does not have a child to deactivate.");
            }
            instances.Add(newBlock.transform);

            count++;

            if (count % spawnPerFrame == 0)
                yield return null;
        }

        // Optional: yield after final batch if not a multiple
        if (count % spawnPerFrame != 0)
            yield return null;
    }


    void SpawnFormation(int idx)
    {
        // Destroy old
        int n = transform.childCount;
        for (int i = n-1; i >= 0; i--)
        {
            DestroyImmediate(transform.GetChild(i).gameObject);
        }
        instances.Clear();

        var positions = positionsPerFormation[idx];

        for (int i = 0; i < positions.Count; i++)
        {
            Vector3 pos = positions[i];
            GameObject prefabToSpawn = null;

            // Is this one of the ring‐centers? (Compare XZ only)
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
                    break;
                }
            }

            // If not a center, pick by radius
            if (prefabToSpawn == null)
                prefabToSpawn = GetWeightedRandomPrefabAtPosition(pos);
            Quaternion randomYRotation = Quaternion.Euler(0f, 90f * Random.Range(0, 4), 0f);
            Transform t = Instantiate(prefabToSpawn, pos + new Vector3(0, transform.position.y, 0), randomYRotation, transform).transform;

            instances.Add(t);
        }
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
        foreach (var pos in list)
        {
            Gizmos.DrawWireCube(pos, Vector3.one * 0.1f);
        }

        // Draw grid outline
        Gizmos.color = Color.yellow;
        Vector3 center = gridCenter;
        float width = gridSpanX;
        float depth = gridSpanZ;
        Vector3 bottomLeft = transform.position + new Vector3(center.x - width / 2f, 0, center.z - depth / 2f);
        Vector3 bottomRight = bottomLeft + new Vector3(width, 0, 0);
        Vector3 topLeft = bottomLeft + new Vector3(0, 0, depth);
        Vector3 topRight = bottomLeft + new Vector3(width, 0, depth);

        Gizmos.DrawLine(bottomLeft, bottomRight);
        Gizmos.DrawLine(bottomRight, topRight);
        Gizmos.DrawLine(topRight, topLeft);
        Gizmos.DrawLine(topLeft, bottomLeft);

        if (radiusPrefabs != null && radiusPrefabs.Count > 0)
        {
            Gizmos.color = Color.green;
            foreach (var rp in radiusPrefabs)
            {
                float radius = rp.radius;
                Vector3 customCenter = transform.position + gridCenter + rp.centerOffset;

                int segments = 64;
                float angleStep = 360f / segments;
                Vector3 prevPoint = customCenter + new Vector3(Mathf.Cos(0), 0, Mathf.Sin(0)) * radius;

                for (int i = 1; i <= segments; i++)
                {
                    float rad = Mathf.Deg2Rad * (i * angleStep);
                    Vector3 nextPoint = customCenter + new Vector3(Mathf.Cos(rad), 0, Mathf.Sin(rad)) * radius;
                    Gizmos.DrawLine(prevPoint, nextPoint);
                    prevPoint = nextPoint;
                }
            }

        }
    }
#endif

#endif
}

using UnityEngine;
using System.Collections.Generic;
using static UnityEngine.Rendering.HableCurve;

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

public class GridFormationController : FormationProvider
{
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

    private float gridSpanX, gridSpanZ; private Vector3 gridCenter;
    private int currentIndex = 0; private bool isTransitioning = false; private float timer = 0f;
    private int difficultyLevel = 1;

    void Start()
    {
        if (!Application.isPlaying) return;

        difficultyLevel = DungeonManager.Instance.DifficultyLevel;
        //DungeonManager.Instance.OnDifficultyChange += HandleDifficultyChange;
        InitializeFormations();
        SpawnFormation(currentIndex);
    }

    //private void HandleDifficultyChange(int difficultyLevel)
    //{
    //    this.difficultyLevel = difficultyLevel;
    //    currentIndex = 0;
    //    InitializeFormations();
    //}

    //public void StartFormation() => SpawnFormation(currentIndex);

    void Update()
    {
        if (!Application.isPlaying || instances.Count == 0 || formations.Count < 2) return;
        if (!isTransitioning && Input.GetKeyDown(KeyCode.Space))
        {
            NextTransition();
        }
        if (isTransitioning)
        {
            timer += Time.deltaTime;
            float t = Mathf.Clamp01(timer / transitionDuration);
            for (int i = 0; i < instances.Count; i++)
                instances[i].localPosition = Vector3.Lerp(transitionStart[i], transitionTarget[i], t);
            if (timer >= transitionDuration)
            {
                isTransitioning = false;
                currentIndex = (currentIndex + 1) % formations[difficultyLevel].config.Count;
            }
        }
    }

    public override void NextTransition()
    {
        isTransitioning = true; timer = 0f;
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

                basePositionsList.Add(new Vector3(rawX, 0, rawZ));

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

                float y = EvaluateFormation(config.type, new Vector3(px, 0, pz), f);
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

    void SpawnFormation(int idx)
    {
        // Destroy old
        foreach (Transform c in transform)
            DestroyImmediate(c.gameObject);
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

            // Instantiate and record
            Transform t = Instantiate(prefabToSpawn, pos, Quaternion.identity, transform).transform;
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

    GameObject GetWeightedRandomPrefab() { float tot = 0; cuboidPrefabs.ForEach(w => tot += w.weight); float r = Random.value * tot, acc = 0f; foreach (var w in cuboidPrefabs) { acc += w.weight; if (r <= acc) return w.prefab; } return cuboidPrefabs[0].prefab; }
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
#if UNITY_EDITOR
#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        if (cuboidPrefabs == null || cuboidPrefabs.Count == 0) return;

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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class PhaseShapeLevel : MonoBehaviour
{
    public enum ShapeType { Web, Pyramid, Star, Castle, Crown,UpSpiral, FlatSpiral, DownSpiral, Box }

    [System.Serializable]
    public struct CubePrefabData
    {
        public GameObject prefab;
        [Range(0, 100)]
        public float spawnChance;
    }

    [System.Serializable]
    public struct PhaseConfig
    {
        public ShapeType shape;
        public int cubeCount;
        public float radius;
        public float jitter;
        public float gapChance;
        public int starPoints;
        public float innerRadius;

        [Range(0f, 1f)]
        public float rotatedPercentage;
        public Vector3 minRotation;
        public Vector3 maxRotation;
    }


    [Header("Phases: 0=Web,1=Pyramid,2=Star (length=3)")]
    public PhaseConfig[] phases;

    [Header("Prefabs")]
    public CubePrefabData[] cubePrefabs;
    public GameObject stonePrefab;
    public GameObject statuePrefab;

    [Header("Transition duration (seconds)")]
    public float transitionTime = 2f;

    [Header("Statue Settings")]
    public float statueStartHeight = 10f; // height at start

    private bool shouldAdvancePhase = false;
    
    private int currentPhase = 0;
    private List<GameObject> spawnedCubes = new List<GameObject>();
    private List<GameObject> stones = new List<GameObject>();
    private GameObject statue;
    private int stonesDestroyed = 0;

    private List<Vector3> castleLayoutPoints = new List<Vector3>();
    private bool castleLayoutGenerated = false;

    void Start()
    {
        // Instantiate stone and statue objects once
        if (stonePrefab != null)
        {
            for (int i = 0; i < 3; i++)
            {
                GameObject s = Instantiate(stonePrefab, Vector3.zero, Quaternion.identity, transform);
                s.SetActive(false);
                var sd = s.AddComponent<StoneDestroy>();
                sd.onDestroyed = () => stonesDestroyed++;
                stones.Add(s);
            }
        }

        if (statuePrefab != null)
        {
            statue = Instantiate(statuePrefab, Vector3.zero, Quaternion.identity, transform);
            statue.transform.localPosition = new Vector3(0, statueStartHeight, 0);
            statue.SetActive(true);
        }

        // Begin the phase sequence
        StartCoroutine(RunPhases());
    }

    void Update()
    {
        // Advance to next phase on Space key press
        if (Input.GetKeyDown(KeyCode.Space))
        {
            // simulate all stones destroyed to skip wait
            if (stones.Count > 0)
                stonesDestroyed = stones.Count;
            else
                shouldAdvancePhase = true;
        }
    }

    IEnumerator RunPhases()
    {
        // Phase 0 setup and stone placement
        SpawnPhase(phases[0]);
        if (stones.Count > 0)
            PositionStones(phases[0]);

        // Loop through subsequent phases
        for (int p = 1; p < phases.Length; p++)
        {
            stonesDestroyed = 0;
            PhaseConfig nextCfg = phases[p];
            int N = spawnedCubes.Count;

            // Cache old and new cube positions
            Vector3[] cubeOld = new Vector3[N];
            Vector3[] cubeNew = new Vector3[N];
            for (int i = 0; i < N; i++)
            {
                cubeOld[i] = spawnedCubes[i].transform.position;
                cubeNew[i] = SamplePoint(nextCfg);
            }

            // Cache old and new stone positions
            int S = stones.Count;
            Vector3[] stoneOld = new Vector3[S];
            Vector3[] stoneNew = new Vector3[S];
            List<Vector3> stoneTargets = ComputeStoneTargets(nextCfg);
            for (int i = 0; i < S; i++)
            {
                stoneOld[i] = stones[i].transform.position;
                GameObject nearest = FindNearest(spawnedCubes, stoneTargets[i]);
                stoneNew[i] = nearest.transform.position + Vector3.up * 0.5f;
            }

            // Wait until all stones are destroyed or space pressed
            // Only wait if stones exist
            if (stones.Count > 0)
            {
                while (stonesDestroyed < stones.Count && !shouldAdvancePhase)
                    yield return null;
            }
            else
            {
                while (!shouldAdvancePhase)
                    yield return null;
            }

            shouldAdvancePhase = false; // Reset for next phase

            // Transition cubes and stones
            yield return StartCoroutine(TransitionObjects(spawnedCubes, cubeNew));
            if (stones.Count > 0)
                yield return StartCoroutine(TransitionObjects(stones, stoneNew));

            // Final phase statue placement
            if (p == phases.Length - 1 && statue != null)
            {
                Vector3 targetPos = FindNearestLocal(spawnedCubes, Vector3.zero).transform.localPosition + new Vector3(0, 0.5f, 0);
                Vector3 startPos = new Vector3(targetPos.x, statueStartHeight, targetPos.z);
                statue.transform.localPosition = startPos;
                yield return StartCoroutine(MoveStatueToPosition(statue, targetPos, transitionTime));
            }

            currentPhase = p;
        }
    }

    void SpawnPhase(PhaseConfig cfg)
    {
        foreach (var c in spawnedCubes) Destroy(c);
        spawnedCubes.Clear();

        int spawned = 0, attempts = 0, maxAttempts = cfg.cubeCount * 10;
        while (spawned < cfg.cubeCount && attempts < maxAttempts)
        {
            attempts++;
            Vector3 localPos = SamplePoint(cfg);
            if (Random.value < cfg.gapChance)
                continue;

            GameObject prefab = GetRandomPrefab();
            if (prefab == null) continue;

            GameObject go = Instantiate(prefab, transform);
            go.transform.localPosition = localPos;

            if (Random.value < cfg.rotatedPercentage)
            {
                // Apply random rotation while preserving prefab's original rotation
                Quaternion originalRot = go.transform.localRotation;
                float rotX = originalRot.eulerAngles.x + Random.Range(cfg.minRotation.x, cfg.maxRotation.x);
                float rotY = originalRot.eulerAngles.y + Random.Range(cfg.minRotation.y, cfg.maxRotation.y);
                float rotZ = originalRot.eulerAngles.z + Random.Range(cfg.minRotation.z, cfg.maxRotation.z);
                go.transform.localRotation = Quaternion.Euler(rotX, rotY, rotZ);
            }

            spawnedCubes.Add(go);
            spawned++;
        }

        if (spawned < cfg.cubeCount)
            Debug.LogWarning($"Only spawned {spawned}/{cfg.cubeCount} cubes after {attempts} attempts.");
    }

    GameObject GetRandomPrefab()
    {
        if (cubePrefabs == null || cubePrefabs.Length == 0)
        {
            Debug.LogError("No cube prefabs assigned!");
            return null;
        }

        // Calculate total spawn chance
        float totalChance = 0;
        foreach (var data in cubePrefabs)
        {
            totalChance += data.spawnChance;
        }

        // If total chance is zero or invalid, default to equal distribution
        if (totalChance <= 0)
        {
            Debug.LogWarning("Total spawn chance is zero or negative. Using equal distribution.");
            int index = Random.Range(0, cubePrefabs.Length);
            return cubePrefabs[index].prefab;
        }

        float randomPoint = Random.Range(0, totalChance);

        float current = 0;
        foreach (var data in cubePrefabs)
        {
            current += data.spawnChance;
            if (randomPoint <= current)
            {
                return data.prefab;
            }
        }

        // Fallback to last prefab if no selection (shouldn't happen)
        return cubePrefabs[cubePrefabs.Length - 1].prefab;
    }

    Vector3 SamplePoint(PhaseConfig cfg)
    {
        switch (cfg.shape)
        {
            case ShapeType.Web: return SampleWeb(cfg);
            case ShapeType.Pyramid: return SamplePyramid(cfg);
            case ShapeType.Star: return SampleStar(cfg);
            case ShapeType.Castle: return SampleCastle(cfg);
            case ShapeType.Crown: return SampleCrown(cfg);
            case ShapeType.UpSpiral: return SampleUpSpiral(cfg);
            case ShapeType.FlatSpiral: return SampleFlatSpiral(cfg);
            case ShapeType.DownSpiral: return SampleDownSpiral(cfg);
            case ShapeType.Box: return SampleBox(cfg);
            default: return Vector3.zero;
        }
    }

    Vector3 SampleWeb(PhaseConfig cfg)
    {
        int spokes = 8;
        int idx = Random.Range(0, spokes);
        float ang = idx * (2f * Mathf.PI / spokes);
        float t = Random.value;
        Vector3 hub = Vector3.zero;
        Vector3 node = new Vector3(cfg.radius * Mathf.Cos(ang), 0, cfg.radius * Mathf.Sin(ang));
        Vector3 pos = Vector3.Lerp(hub, node, t);
        pos += Vector3.Cross((node - hub).normalized, Vector3.up) * RandomGaussian(0, cfg.jitter);
        return pos;
    }

    Vector3 SamplePyramid(PhaseConfig cfg)
    {
        Vector3[] V = new Vector3[] {
            new Vector3(0, 0, cfg.radius),
            new Vector3(cfg.radius * Mathf.Sin(60f * Mathf.Deg2Rad), 0, -cfg.radius * 0.5f),
            new Vector3(-cfg.radius * Mathf.Sin(60f * Mathf.Deg2Rad), 0, -cfg.radius * 0.5f),
            new Vector3(0, cfg.radius, 0)
        };
        int i = Random.Range(0, 4);
        int j = (i + Random.Range(1, 4)) % 4;
        Vector3 a = V[i], b = V[j];
        float t = Random.value;
        Vector3 pos = Vector3.Lerp(a, b, t);
        Vector3 dir = (b - a).normalized;
        pos += Vector3.Cross(dir, Vector3.up).normalized * RandomGaussian(0, cfg.jitter);
        pos += Vector3.Cross(dir, Vector3.Cross(dir, Vector3.up)).normalized * RandomGaussian(0, cfg.jitter);
        return pos;
    }

    Vector3 SampleStar(PhaseConfig cfg)
    {
        int pts = Mathf.Max(5, cfg.starPoints);
        int idx = Random.Range(0, pts);
        float ang = idx * (2f * Mathf.PI / pts);
        float r = Random.Range(cfg.innerRadius, cfg.radius);
        Vector3 pos = new Vector3(r * Mathf.Cos(ang), 0, r * Mathf.Sin(ang));
        Vector3 jitter = Random.onUnitSphere * cfg.jitter;
        jitter.y = 0;
        return pos + jitter;
    }

    Vector3 SampleCastle(PhaseConfig cfg)
    {
        if (!castleLayoutGenerated)
        {
            castleLayoutPoints.Clear();
            int numColumns = 40;
            float blockSpacing = 2f;
            float blockHeightUnit = 1.5f;
            int[] possibleHeights = { 0, 1, 3, 5 };
            int lastHeight = -10;

            for (int i = 0; i < numColumns; i++)
            {
                int newHeight;
                do
                {
                    newHeight = possibleHeights[Random.Range(0, possibleHeights.Length)];
                } while (Mathf.Abs(newHeight - lastHeight) < 2); // high contrast
                lastHeight = newHeight;

                for (int h = 0; h < newHeight; h++)
                {
                    Vector3 pos = new Vector3(i * blockSpacing, h * blockHeightUnit, 0);
                    pos += Random.insideUnitSphere * cfg.jitter;
                    pos.y = Mathf.Max(0, pos.y); // prevent blocks going under
                    castleLayoutPoints.Add(pos);
                }
            }

            castleLayoutGenerated = true;
        }

        // Return a random precomputed castle block
        return castleLayoutPoints[Random.Range(0, castleLayoutPoints.Count)];
    }

    Vector3 SampleCrown(PhaseConfig cfg)
    {
        // Peaks arranged vertically like a crown or mountains
        float spacing = cfg.radius / cfg.cubeCount;
        int idx = Random.Range(0, cfg.cubeCount);
        float x = idx * spacing - cfg.radius / 2;
        float height = Mathf.Sin((float)idx / cfg.cubeCount * Mathf.PI) * cfg.radius;
        return new Vector3(x, height, 0) + Random.insideUnitSphere * cfg.jitter;
    }

    Vector3 SampleUpSpiral(PhaseConfig cfg)
    {
        float turns = 3f; // Number of spiral turns
        float spacing = cfg.radius / cfg.cubeCount; // Spiral tightness
        float heightRange = cfg.radius; // Vertical extent of the spiral

        // Get an index and corresponding angle
        int index = Random.Range(0, cfg.cubeCount);
        float t = (float)index / cfg.cubeCount;
        float angle = t * turns * 2f * Mathf.PI;

        // Spiral radius (could shrink or grow)
        float spiralRadius = t * cfg.radius;

        // Base position in spiral
        float x = spiralRadius * Mathf.Cos(angle);
        float z = spiralRadius * Mathf.Sin(angle);
        float y = Mathf.Lerp(heightRange / 2f, -heightRange / 2f, t); 


        // Add jitter for randomness
        Vector3 jitter = Random.insideUnitSphere * cfg.jitter;
        return new Vector3(x, y, z) + jitter;
    }

    Vector3 SampleFlatSpiral(PhaseConfig cfg)
    {
        float turns = 3f; // Number of spiral turns
        float spacing = cfg.radius / cfg.cubeCount; // Spiral tightness
        float heightRange = cfg.radius; // Vertical extent of the spiral

        // Get an index and corresponding angle
        int index = Random.Range(0, cfg.cubeCount);
        float t = (float)index / cfg.cubeCount;
        float angle = t * turns * 2f * Mathf.PI;

        // Spiral radius (could shrink or grow)
        float spiralRadius = t * cfg.radius;

        // Base position in spiral
        float x = spiralRadius * Mathf.Cos(angle);
        float z = spiralRadius * Mathf.Sin(angle);
        float y = 0f;


        // Add jitter for randomness
        Vector3 jitter = Random.insideUnitSphere * cfg.jitter;
        return new Vector3(x, y, z) + jitter;
    }

    Vector3 SampleDownSpiral(PhaseConfig cfg)
    {
        float turns = 3f; // Number of spiral turns
        float spacing = cfg.radius / cfg.cubeCount; // Spiral tightness
        float heightRange = cfg.radius; // Vertical extent of the spiral

        // Get an index and corresponding angle
        int index = Random.Range(0, cfg.cubeCount);
        float t = (float)index / cfg.cubeCount;
        float angle = t * turns * 2f * Mathf.PI;

        // Spiral radius (could shrink or grow)
        float spiralRadius = t * cfg.radius;

        // Base position in spiral
        float x = spiralRadius * Mathf.Cos(angle);
        float z = spiralRadius * Mathf.Sin(angle);
        float y = Mathf.Lerp(-heightRange / 2f, heightRange / 2f, t);

        // Add jitter for randomness
        Vector3 jitter = Random.insideUnitSphere * cfg.jitter;
        return new Vector3(x, y, z) + jitter;
    }

    Vector3 SampleBox(PhaseConfig cfg)
    {
        int columns = Mathf.CeilToInt(Mathf.Sqrt(cfg.cubeCount));
        float spacing = cfg.radius / columns;

        int index = Random.Range(0, cfg.cubeCount);
        int x = index % columns;
        int z = index / columns;

        float xPos = (x - columns / 2f) * spacing;
        float zPos = (z - columns / 2f) * spacing;

        float yJitter = Random.Range(-cfg.jitter, cfg.jitter);
        Vector3 basePos = new Vector3(xPos, yJitter, zPos);

        return basePos;
    }

    List<Vector3> ComputeStoneTargets(PhaseConfig cfg)
    {
        var targets = new List<Vector3>();
        if (cfg.shape == ShapeType.Web)
        {
            int[] picks = { 0, 2, 4 };
            int spokes = 8;
            foreach (int p in picks)
                targets.Add(new Vector3(cfg.radius * Mathf.Cos(p * 2f * Mathf.PI / spokes), 0,
                                        cfg.radius * Mathf.Sin(p * 2f * Mathf.PI / spokes)));
        }
        else if (cfg.shape == ShapeType.Pyramid)
        {
            targets.Add(new Vector3(0, 0, cfg.radius));
            targets.Add(new Vector3(cfg.radius * Mathf.Sin(60f * Mathf.Deg2Rad), 0, -cfg.radius * 0.5f));
            targets.Add(new Vector3(-cfg.radius * Mathf.Sin(60f * Mathf.Deg2Rad), 0, -cfg.radius * 0.5f));
        }
        else
        {
            int pts = Mathf.Max(5, cfg.starPoints);
            int[] picks = { 0, pts / 2, pts - 1 };
            foreach (int p in picks)
                targets.Add(new Vector3(cfg.radius * Mathf.Cos(p * 2f * Mathf.PI / pts), 0,
                                        cfg.radius * Mathf.Sin(p * 2f * Mathf.PI / pts)));
        }
        return targets;
    }

    IEnumerator TransitionObjects(List<GameObject> objs, Vector3[] endPos)
    {
        float startTime = Time.time;
        float endTime = startTime + transitionTime;

        while (Time.time < endTime)
        {
            // how much of this frame to blend, so that we'll exactly hit the target by endTime
            float dt = Time.deltaTime;
            float remaining = endTime - Time.time;
            float blend = dt / (remaining + dt);

            for (int i = 0; i < objs.Count; i++)
            {
                // always lerp from *current* position to the fixed endPos
                objs[i].transform.localPosition =
                    Vector3.Lerp(objs[i].transform.localPosition, endPos[i], blend);
            }

            yield return null;
        }

        // ensure everyone ends exactly on target
        for (int i = 0; i < objs.Count; i++)
            objs[i].transform.localPosition = endPos[i];
    }


    void PositionStones(PhaseConfig cfg)
    {
        var targets = ComputeStoneTargets(cfg);
        for (int i = 0; i < stones.Count; i++)
        {
            GameObject nearest = FindNearestLocal(spawnedCubes, targets[i]);
            Vector3 offset = new Vector3(0f, 0.5f, 0f);
            stones[i].transform.localPosition = nearest.transform.localPosition + offset;
            stones[i].SetActive(true);
        }
    }

    void PositionStatue(PhaseConfig cfg)
    {
        GameObject nearest = FindNearestLocal(spawnedCubes, Vector3.zero);
        Vector3 offset = new Vector3(0f, 0.5f, 0f);
        statue.transform.localPosition = nearest.transform.localPosition + offset;
        statue.SetActive(true);
    }

    IEnumerator MoveStatueToPosition(GameObject obj, Vector3 targetPos, float duration)
    {
        Vector3 startPos = obj.transform.localPosition;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            obj.transform.localPosition = Vector3.Lerp(startPos, targetPos, t);
            yield return null;
        }

        obj.transform.localPosition = targetPos;
    }

    GameObject FindNearest(List<GameObject> list, Vector3 pt)
    {
        GameObject best = null;
        float bestD = float.MaxValue;
        foreach (var go in list)
        {
            float d = (go.transform.position - pt).sqrMagnitude;
            if (d < bestD)
            {
                bestD = d;
                best = go;
            }
        }
        return best;
    }

    GameObject FindNearestLocal(List<GameObject> list, Vector3 localPt)
    {
        GameObject best = null;
        float bestD = float.MaxValue;
        foreach (var go in list)
        {
            float d = (go.transform.localPosition - localPt).sqrMagnitude;
            if (d < bestD)
            {
                bestD = d;
                best = go;
            }
        }
        return best;
    }

    float RandomGaussian(float mu, float sigma)
    {
        float u1 = 1f - Random.value;
        float u2 = 1f - Random.value;
        return mu + sigma * Mathf.Sqrt(-2f * Mathf.Log(u1)) * Mathf.Sin(2f * Mathf.PI * u2);
    }

    public void TriggerPhaseChange()
    {
        shouldAdvancePhase = true;
    }
}

public class StoneDestroy : MonoBehaviour
{
    public System.Action onDestroyed;
    void OnDestroy() => onDestroyed?.Invoke();
}

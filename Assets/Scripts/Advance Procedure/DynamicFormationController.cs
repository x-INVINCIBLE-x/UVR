using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

// 1) Define an enum listing all possible shapes
public enum FormationType
{
    Sphere,
    Arrow,
    Clock,
    JapaneseUmbrella,
    Ring,
    FantasyClock,
    Eye
    // … add more as you create new generators …
}

// Skip Index Number ZERO
public class DynamicFormationController : FormationProvider
{
    [System.Serializable]
    public class FormationContainer
    {
        public int difficulty = 1;
        public List<FormationType> formationTypes;
    }

    [Header("General")]
    public GameObject cubePrefab;
    public int cubeCount = 300;
    public bool shouldRotate = false;
    public bool inverseRotation = false;
    public float rotationSpeed;
    public float jitterAmount = 0.2f;

    [Header("Timing")]
    public float formationLerpTime = 2f;
    public float delayPerCube = 0.02f;

    [Header("Scatter Settings")]
    public float startRadius = 30f;
    public float startHeight = 50f;

    [Header("Vortex Settings")]
    public float vortexRadius = 40f;
    public float vortexHeight = 10f;
    public float vortexSpeed = 90f;
    public float verticalOscillation = 2f;
    public float tornadoInnerRadius = 10f;
    public float tornadoWallOffset = 5f;

    [Header("Tick-Mark Formation")]
    [Tooltip("How many radial ticks to draw (e.g. 8 for hour-marks).")]
    public int tickCount = 8;

    [Tooltip("Length of each tick segment (how far it extends inwards/outwards from formationRadius).")]
    public float tickLength = 2f;
    [Tooltip("Angular width of each tick (in degrees).")]
    public float tickAngularWidth = 5f;

    [Header("Formation Settings")]
    public float formationRadius = 15f;

    // ─────────────────────────────────────────────────────────────────────────
    // Japanese Umbrella parameters (if you use that generator)
    [Header("Japanese Umbrella Span")]
    [Range(10f, 90f)] public float umbrellaSpanDegrees = 72f;
    [Header("Japanese Umbrella Ribs")]
    [Range(0f, 1f)] public float canopyRatio = 0.75f;
    public int ribCount = 12;
    [Range(0f, 0.5f)] public float ribStrength = 0.15f;
    [Header("Japanese Umbrella Radius & Handle")]
    [Min(0f)] public float umbrellaRadius = 15f;
    [Min(0f)] public float handleLength = 10f;
    // ─────────────────────────────────────────────────────────────────────────

    [Header("Arrow Settings")]

    // Fraction of cubes that form the vertical shaft (0.1 → 0.9)
    [Range(0.1f, 0.9f)]
    public float shaftRatio = 0.3f;

    // How many cubes to add to the head width on each successive row (odd integers only: 1, 3, 5, …)
    [Tooltip("Growth in head width per row (must be an odd positive integer, e.g. 1, 3, 5)")]
    public int headWidthStep = 3;

    // Override the overall “grid” aspect if you like.
    // If <= 0, we auto-compute w × h from cubeCount; otherwise we force w = gridWidth.
    [Min(0)]
    public int gridWidthOverride = 0;

    // If gridWidthOverride > 0, you can also manually set gridHeightOverride.
    // If <= 0, it’s computed from cubeCount & gridWidthOverride.
    [Min(0)]
    public int gridHeightOverride = 0;

    // 2) Expose a public formations of FormationType so you can pick the sequence in Inspector
    [Header("Pick Your Sequence of Formations")]
    public List<FormationContainer> formationSequence = new List<FormationContainer>();

    // Internal formations of delegate pointers (built at runtime from the enum formations)
    private List<FormationGen> formations = new List<FormationGen>();
    private int currentFormationIndex = 0;

    private delegate Vector3[] FormationGen();

    private List<GameObject> cubes = new List<GameObject>();
    private Vector3[] finalPositions;
    private Vector3 formationCenter;
    private bool isAnimating = false;

    private float[] orbitAngles;
    private float[] orbitHeights;
    private float[] randomOscillationOffsets;
    private float[] cubeTimers;
    private float transitionStartTime;
    private enum CubeState { Orbiting, Transitioning, Done }
    private CubeState[] cubeStates;
    private float[] transitionStartT;
    private int rotationMultiplier = 1;

    public event System.Action OnFormationStart;
    public event System.Action<FormationType> OnFormationComplete;
    public event System.Action<FormationType> OnUnwrapStart;
    public event System.Action OnUnwrapComplete;

    private int difficultyLevel = 1;

#if UNITY_EDITOR
    private void OnValidate()
    {
        for (int i = 0; i < formationSequence.Count; i++)
        {
            formationSequence[i].difficulty = i + 1;
        }
    }
#endif

    private void OnEnable()
    {
        FormationConsequenceManager.Register(this);
    }

    void Start()
    {
        difficultyLevel = DungeonManager.Instance.DifficultyLevel - 1;

        if (difficultyLevel >= formationSequence.Count)
        {
            Debug.Log("<color=cyan>  " + gameObject.name + "</color> <color=green> has no data for CURRENT DIFFICULTY. Reverting to LAST DIFFICULTY DATA </color>");
            difficultyLevel = formationSequence.Count - 1;
        }

        Initialize();

        StartFormation();
    }

    private void Initialize()
    {
        // Initialize per‐cube arrays
        orbitAngles = new float[cubeCount];
        orbitHeights = new float[cubeCount];
        randomOscillationOffsets = new float[cubeCount];
        cubeTimers = new float[cubeCount];
        cubeStates = new CubeState[cubeCount];
        transitionStartT = new float[cubeCount];
        rotationMultiplier = inverseRotation ? -1 : 1;

        // Instantiate cubes in a scattered “start” ring
        for (int i = 0; i < cubeCount; i++)
        {
            float angle = (float)i / cubeCount * Mathf.PI * 2f;
            Vector3 pos = new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) * startRadius;
            pos.y = startHeight;
            GameObject cube = Instantiate(cubePrefab, pos, Quaternion.identity, transform);
            cubes.Add(cube);

            orbitAngles[i] = Random.Range(0f, 360f);
            orbitHeights[i] = Random.Range(0.8f, 1.2f);
            randomOscillationOffsets[i] = Random.Range(0f, 2f * Mathf.PI);
        }

        // Build the internal “formations” formations based on the enum formations from Inspector
        formations.Clear();
        foreach (var type in formationSequence[difficultyLevel].formationTypes)
        {
            switch (type)
            {
                case FormationType.Sphere:
                    formations.Add(GenerateSphere);
                    break;
                case FormationType.Arrow:
                    formations.Add(GenerateArrow);
                    break;
                case FormationType.Clock:
                    formations.Add(GenerateClock);
                    break;
                case FormationType.JapaneseUmbrella:
                    formations.Add(GenerateJapaneseUmbrella);
                    break;
                case FormationType.Ring:
                    formations.Add(GenerateRing);
                    break;
                case FormationType.FantasyClock:
                    formations.Add(GenerateFantasyClock);
                    break;
                case FormationType.Eye:
                    formations.Add(GenerateFantasyEye);
                    break;

                    // If you add new FormationType entries, handle them here…
            }
        }
    }

    private void StartFormation()
    {
        // If the user left the Inspector formations empty, at least default to Sphere
        if (formations.Count == 0)
        {
            Debug.LogWarning("No scalingFormations selected! Defaulting to Sphere.");
            formations.Add(GenerateSphere);
        }

        // Compute the very first formation and center
        finalPositions = formations[currentFormationIndex]();
        formationCenter = CalculateFormationCenter(finalPositions);

        // Start the initial formation transition
        StartCoroutine(TransitionFormation());
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && !isAnimating)
        {
            // Advance to next formation in our formations, wrap around if needed
            NextTransition();
        }

        if (shouldRotate && !isAnimating )
        {
            transform.Rotate(Vector3.up * rotationMultiplier, rotationSpeed * Time.deltaTime);
        }
    }

    public override void NextTransition()
    {
        currentFormationIndex = (currentFormationIndex + 1) % formationSequence[difficultyLevel].formationTypes.Count;
        StartCoroutine(FormationToVortexThenToNextFormation());
    }

    // ─────────────────────────────────────────────────────────────────────────
    // Coroutine: smoothly spin into vortex, then peel off into next formation
    // ─────────────────────────────────────────────────────────────────────────
    IEnumerator FormationToVortexThenToNextFormation()
    {
        isAnimating = true;
        float now = Time.time;

        int count = formationSequence[difficultyLevel].formationTypes.Count;
        int prevIndex = (currentFormationIndex - 1 + count) % count;

        OnUnwrapStart?.Invoke(formationSequence[difficultyLevel].formationTypes[prevIndex]);

        // Get the next‐formation positions & center
        Vector3[] nextFormationPositions = formations[currentFormationIndex]();
        Vector3 nextCenter = CalculateFormationCenter(nextFormationPositions);

        // Capture “start” for each cube and compute each cube’s final orbit parameters
        Vector3[] startPositions = new Vector3[cubeCount];
        float[] targetHeights = new float[cubeCount];
        float[] targetRadii = new float[cubeCount];
        float[] vortexStartTimes = new float[cubeCount];
        float minRadius = formationRadius + tornadoInnerRadius + tornadoWallOffset;

        for (int i = 0; i < cubeCount; i++)
        {
            startPositions[i] = cubes[i].transform.position;

            float tNorm = (float)i / cubeCount;
            targetHeights[i] = Mathf.Lerp(vortexHeight, startHeight, tNorm);
            targetRadii[i] = Mathf.Lerp(minRadius, vortexRadius, tNorm);

            vortexStartTimes[i] = now + i * delayPerCube;
            cubeStates[i] = CubeState.Transitioning;

            orbitAngles[i] = Random.Range(0f, 360f);
        }

        // PHASE A: “Unravel into vortex”
        bool allInVortex = false;
        while (!allInVortex)
        {
            allInVortex = true;
            float tNow = Time.time;

            for (int i = 0; i < cubeCount; i++)
            {
                if (tNow < vortexStartTimes[i])
                {
                    allInVortex = false;
                    continue;
                }

                float t = Mathf.Clamp01((tNow - vortexStartTimes[i]) / formationLerpTime);

                // Advance the angle so cube “orbits”
                orbitAngles[i] += vortexSpeed * Time.deltaTime;
                float angRad = orbitAngles[i] * Mathf.Deg2Rad;

                Vector3 spiralXZ = new Vector3(
                    Mathf.Cos(angRad),
                    0f,
                    Mathf.Sin(angRad)
                ) * targetRadii[i];

                float yOsc = Mathf.Sin(Time.time * 2f + randomOscillationOffsets[i]) * verticalOscillation;
                Vector3 vortexTarget = formationCenter + spiralXZ + Vector3.up * (targetHeights[i] + yOsc);

                cubes[i].transform.position = Vector3.Lerp(startPositions[i], vortexTarget, t);

                if (t < 1f) allInVortex = false;
                else cubeStates[i] = CubeState.Orbiting;
            }
            yield return null;
        }

        // Record each cube’s exact “in‐vortex” position
        Vector3[] vortexPositions = new Vector3[cubeCount];
        for (int i = 0; i < cubeCount; i++)
        {
            float angRad = orbitAngles[i] * Mathf.Deg2Rad;
            Vector3 spiralXZ = new Vector3(
                Mathf.Cos(angRad),
                0f,
                Mathf.Sin(angRad)
            ) * targetRadii[i];

            float yOsc = Mathf.Sin(Time.time * 2f + randomOscillationOffsets[i]) * verticalOscillation;
            vortexPositions[i] = formationCenter + spiralXZ + Vector3.up * (targetHeights[i] + yOsc);
        }

        // PHASE B + C: “Keep spinning in vortex while cubes peel off”
        float[] peelStartTimes = new float[cubeCount];
        for (int i = 0; i < cubeCount; i++)
        {
            peelStartTimes[i] = Time.time + i * delayPerCube;
            cubeStates[i] = CubeState.Transitioning;
        }

        bool allArrived = false;
        while (!allArrived)
        {
            allArrived = true;
            float tNow = Time.time;

            for (int i = 0; i < cubeCount; i++)
            {
                // Always advance orbiting angle so the vortex continues to spin
                orbitAngles[i] += vortexSpeed * Time.deltaTime;
                float angRad = orbitAngles[i] * Mathf.Deg2Rad;

                if (tNow < peelStartTimes[i])
                {
                    // This cube has not peeled off → remain in full vortex orbit
                    Vector3 spiralXZ = new Vector3(
                        Mathf.Cos(angRad),
                        0f,
                        Mathf.Sin(angRad)
                    ) * targetRadii[i];

                    float yOsc = Mathf.Sin(Time.time * 2f + randomOscillationOffsets[i]) * verticalOscillation;
                    float yBase = targetHeights[i];

                    cubes[i].transform.position = formationCenter + spiralXZ + Vector3.up * (yBase + yOsc);
                    allArrived = false;
                }
                else
                {
                    // Peel off: interpolate from vortexPositions[i] -> nextFormationPositions[i]
                    float u = Mathf.Clamp01((tNow - peelStartTimes[i]) / formationLerpTime);

                    // Decay swirl radius
                    float swirlRadiusNow = Mathf.Lerp(targetRadii[i], 0f, u);
                    Vector3 swirlXZ = new Vector3(
                        Mathf.Cos(angRad),
                        0f,
                        Mathf.Sin(angRad)
                    ) * swirlRadiusNow;

                    // Decay vertical oscillation
                    float yOsc = Mathf.Sin(Time.time * 2f + randomOscillationOffsets[i])
                                 * verticalOscillation * (1f - u);

                    // Base LERP from vortexPositions -> next formation
                    Vector3 basePos = Vector3.Lerp(
                        vortexPositions[i],
                        nextCenter + (nextFormationPositions[i] - nextCenter),
                        u
                    );

                    cubes[i].transform.position = basePos + swirlXZ + Vector3.up * yOsc;

                    if (u < 1f) allArrived = false;
                    else cubeStates[i] = CubeState.Done;
                }
            }

            yield return null;
        }

        // Update for next cycle
        formationCenter = nextCenter;
        finalPositions = nextFormationPositions;

        OnFormationComplete?.Invoke(formationSequence[difficultyLevel].formationTypes[currentFormationIndex]);
        isAnimating = false;
    }

    // ─────────────────────────────────────────────────────────────────────────
    // Coroutine: smoothly move from “wherever cubes are” → finalPositions[]
    // ─────────────────────────────────────────────────────────────────────────
    IEnumerator TransitionFormation()
    {
        isAnimating = true;
        transitionStartTime = Time.time;
        OnFormationStart?.Invoke();

        for (int i = 0; i < cubeCount; i++)
        {
            if (cubeStates[i] == CubeState.Done)
            {
                cubeStates[i] = CubeState.Transitioning;
                cubeTimers[i] = i * delayPerCube;
                transitionStartT[i] = 0f;
            }
            else
            {
                cubeStates[i] = CubeState.Orbiting;
                cubeTimers[i] = i * delayPerCube;
                transitionStartT[i] = 0f;
            }
        }

        while (true)
        {
            float timeSinceStart = Time.time - transitionStartTime;
            bool allDone = true;

            for (int i = 0; i < cubeCount; i++)
            {
                switch (cubeStates[i])
                {
                    case CubeState.Orbiting:
                        if (timeSinceStart >= cubeTimers[i])
                        {
                            cubeStates[i] = CubeState.Transitioning;
                            transitionStartT[i] = timeSinceStart;
                        }
                        else
                        {
                            // Gentle orbit until it’s time to move
                            orbitAngles[i] += vortexSpeed * Time.deltaTime;
                            float rad = orbitAngles[i] * Mathf.Deg2Rad;

                            float y = vortexHeight + Mathf.Sin(Time.time * 2f + randomOscillationOffsets[i])
                                      * verticalOscillation * orbitHeights[i];
                            float normalizedHeight = Mathf.InverseLerp(vortexHeight, startHeight, Mathf.Max(y, vortexHeight + 0.01f));

                            float minRad = formationRadius + tornadoInnerRadius + tornadoWallOffset;
                            float tornadoRad = Mathf.Lerp(minRad, vortexRadius, normalizedHeight);

                            Vector3 horizontal = new Vector3(Mathf.Cos(rad), 0, Mathf.Sin(rad)) * tornadoRad;
                            cubes[i].transform.position = formationCenter + horizontal + Vector3.up * y;
                        }
                        allDone = false;
                        break;

                    case CubeState.Transitioning:
                        float t = (timeSinceStart - transitionStartT[i]) / formationLerpTime;
                        if (t >= 1f)
                        {
                            cubes[i].transform.position = finalPositions[i];
                            cubeStates[i] = CubeState.Done;
                        }
                        else
                        {
                            Vector3 currentPos = cubes[i].transform.position;
                            Vector3 target = finalPositions[i];
                            cubes[i].transform.position = Vector3.Lerp(currentPos, target, t);
                        }
                        allDone = false;
                        break;

                    case CubeState.Done:
                        cubes[i].transform.position = finalPositions[i];
                        break;
                }
            }

            if (allDone) break;
            yield return null;
        }

        OnFormationComplete?.Invoke(formationSequence[difficultyLevel].formationTypes[currentFormationIndex]);
        isAnimating = false;
    }

    // ─────────────────────────────────────────────────────────────────────────
    // Formation Generators
    // ─────────────────────────────────────────────────────────────────────────
    Vector3[] GenerateSphere()
    {
        Vector3[] positions = new Vector3[cubeCount];
        float r = formationRadius;

        for (int i = 0; i < cubeCount; i++)
        {
            float phi = Mathf.Acos(1 - 2f * (i + 0.5f) / cubeCount);
            float theta = Mathf.PI * (1 + Mathf.Sqrt(5f)) * (i + 0.5f);
            positions[i] = transform.position + new Vector3(
                r * Mathf.Sin(phi) * Mathf.Cos(theta),
                r * Mathf.Sin(phi) * Mathf.Sin(theta),
                r * Mathf.Cos(phi)
            );
        }

        return positions;
    }

    Vector3[] GenerateRing()
    {
        Vector3[] positions = new Vector3[cubeCount];
        float angleStep = 360f / cubeCount;
        float r = formationRadius;

        for (int i = 0; i < cubeCount; i++)
        {
            float angleRad = Mathf.Deg2Rad * angleStep * i;
            positions[i] = transform.position + new Vector3(
                Mathf.Cos(angleRad) * r,
                0f,
                Mathf.Sin(angleRad) * r
            );
        }

        return ApplyJitter(positions);
    }

    Vector3[] GenerateArrow()
    {
        Vector3[] positions = new Vector3[cubeCount];

        // 1) Compute grid size (w × h)
        int w, h;
        if (gridWidthOverride > 0)
        {
            w = gridWidthOverride;
            h = (gridHeightOverride > 0)
                ? gridHeightOverride
                : Mathf.CeilToInt((float)cubeCount / w);
        }
        else
        {
            w = Mathf.CeilToInt(Mathf.Sqrt(cubeCount));
            h = Mathf.CeilToInt((float)cubeCount / w);
        }

        float cx = (w - 1f) / 2f;
        float cy = (h - 1f) / 2f;

        int shaftCount = Mathf.CeilToInt(cubeCount * shaftRatio);
        shaftCount = Mathf.Clamp(shaftCount, 1, cubeCount - 1);
        int headCount = cubeCount - shaftCount;

        int centerX = Mathf.FloorToInt(cx);

        int shaftStartY = Mathf.FloorToInt(cy - (shaftCount - 1) / 2f);
        int shaftEndY = shaftStartY + shaftCount - 1;
        shaftStartY = Mathf.Clamp(shaftStartY, 0, h - 1);
        shaftEndY = Mathf.Clamp(shaftEndY, 0, h - 1);

        int placed = 0;
        for (int y = shaftStartY; y <= shaftEndY && placed < shaftCount; y++)
        {
            positions[placed++] = transform.position + new Vector3((centerX - cx), (y - cy), 0f);
        }

        int headPlaced = 0;
        int rowY = shaftEndY + 1;
        int currentWidth = headWidthStep;
        if (currentWidth < 1 || currentWidth % 2 == 0) currentWidth = 1;

        while (headPlaced < headCount && rowY < h)
        {
            int half = (currentWidth - 1) / 2;
            for (int dx = -half; dx <= half && headPlaced < headCount; dx++)
            {
                int gridX = centerX + dx;
                if (gridX >= 0 && gridX < w)
                {
                    positions[placed++] = new Vector3((gridX - cx), (rowY - cy), 0f);
                    headPlaced++;
                }
            }
            currentWidth += headWidthStep;
            rowY++;
        }

        while (placed < cubeCount)
        {
            positions[placed++] = new Vector3((centerX - cx), (rowY - cy), 0f);
        }

        return positions;
    }

    Vector3[] GenerateClock()
    {
        Vector3[] positions = new Vector3[cubeCount];

        // 1) Number of ticks (radial blocks around the circle)
        int N = Mathf.Max(1, tickCount);

        // 2) Decide how many cubes per tick (distribute evenly, plus any remainder)
        int basePerTick = cubeCount / N;
        int remainder = cubeCount % N;

        // tickCounts[i] = how many cubes belong to the i-th tick
        int[] tickCounts = new int[N];
        for (int i = 0; i < N; i++)
        {
            tickCounts[i] = basePerTick + (i < remainder ? 1 : 0);
        }

        // 3) Precompute some helpful values:
        float halfLen = tickLength * 0.5f;                           // radial “half‐height” of each cuboid
        float R = formationRadius;                             // base radius
        float halfAng = (tickAngularWidth * Mathf.Deg2Rad) * 0.5f;   // half angular width in radians

        int idx = 0;

        // 4) For each tick (i = 0 … N-1):
        for (int i = 0; i < N; i++)
        {
            int countHere = tickCounts[i];
            float centerAngle = (i / (float)N) * Mathf.PI * 2f;  // angle around the circle

            // Compute an approximate “aspect ratio” for the cuboid:
            //    radialSpan = tickLength
            //    angularArcLength ≈ R * (2 * halfAng)
            // We want a roughly square-ish grid, so:
            float radialSpan = tickLength;
            float angularSpan = R * (2f * halfAng);

            // Determine grid resolution: how many steps along radial vs. angular
            // We solve: rows * cols >= countHere, and (rows/cols) ~ (radialSpan/angularSpan)
            int rows = 1;
            int cols = countHere;

            if (countHere > 1)
            {
                float ratio = radialSpan / angularSpan;
                // rows ≈ sqrt(countHere * ratio)
                rows = Mathf.CeilToInt(Mathf.Sqrt(countHere * Mathf.Clamp(ratio, 0.001f, Mathf.Infinity)));
                rows = Mathf.Max(1, rows);

                // Then cols = ceil(countHere / rows)
                cols = Mathf.CeilToInt(countHere / (float)rows);
            }

            // 5) Now lay out countHere cubes in a rows×cols grid,
            //    where:
            //      - “row index” r ∈ [0..rows-1] maps to radial offset
            //      - “col index” c ∈ [0..cols-1] maps to angular offset
            for (int r = 0; r < rows && idx < cubeCount; r++)
            {
                // normalized radial parameter uR ∈ [0..1]
                float uR = (rows == 1) ? 0.5f : (r / (float)(rows - 1));
                // radialDistance runs from [R - halfLen .. R + halfLen]
                float radialDistance = Mathf.Lerp(R - halfLen, R + halfLen, uR);

                for (int c = 0; c < cols && idx < cubeCount; c++)
                {
                    if (idx >= cubeCount) break;

                    // normalized angular parameter uA ∈ [0..1]
                    float uA = (cols == 1) ? 0.5f : (c / (float)(cols - 1));
                    // angular offset from centerAngle: ∈ [–halfAng .. +halfAng]
                    float angOffset = Mathf.Lerp(-halfAng, +halfAng, uA);
                    float angle = centerAngle + angOffset;

                    // Convert polar → cartesian
                    float x = Mathf.Cos(angle) * radialDistance;
                    float z = Mathf.Sin(angle) * radialDistance;

                    positions[idx++] = transform.position + new Vector3(x, 0f, z);
                }
            }
        }

        return positions;
    }

    Vector3[] GenerateJapaneseUmbrella()
    {
        Vector3[] positions = new Vector3[cubeCount];
        float r = umbrellaRadius;

        int canopyCount = Mathf.CeilToInt(cubeCount * canopyRatio);
        int handleCount = cubeCount - canopyCount;

        float φMax = Mathf.Deg2Rad * umbrellaSpanDegrees;
        float minCanopyY = float.MaxValue;

        for (int i = 0; i < canopyCount; i++)
        {
            float u = (canopyCount == 1 ? 1f : (float)i / (canopyCount - 1));
            float φ = u * φMax;
            float θ = i * Mathf.PI * (1f + Mathf.Sqrt(5f));

            float x0 = r * Mathf.Sin(φ) * Mathf.Cos(θ);
            float y0 = r * Mathf.Cos(φ);
            float z0 = r * Mathf.Sin(φ) * Mathf.Sin(θ);
            Vector3 canopyPoint = new Vector3(x0, y0, z0);

            float θnorm = θ % (2f * Mathf.PI);
            float bestDelta = float.MaxValue;
            for (int rib = 0; rib < ribCount; rib++)
            {
                float θrib = (rib / (float)ribCount) * (2f * Mathf.PI);
                float d = Mathf.Min(
                    Mathf.Abs(θnorm - θrib),
                    2f * Mathf.PI - Mathf.Abs(θnorm - θrib)
                );
                if (d < bestDelta) bestDelta = d;
            }

            float ribFactor = Mathf.Clamp01(1f - (bestDelta / (Mathf.PI / ribCount)));
            Vector3 ribNudge = canopyPoint.normalized * (ribStrength * ribFactor * r);
            Vector3 finalCanopyPoint = canopyPoint + ribNudge;
            positions[i] = transform.position + finalCanopyPoint;

            if (finalCanopyPoint.y < minCanopyY)
                minCanopyY = finalCanopyPoint.y;
        }

        for (int j = 0; j < handleCount; j++)
        {
            float t = (handleCount == 1 ? 1f : (float)j / (handleCount - 1));
            float y = Mathf.Lerp(minCanopyY, minCanopyY - handleLength, t);
            positions[canopyCount + j] = transform.position + new Vector3(0f, y, 0f);
        }

        return positions;
    }
    Vector3[] GenerateFantasyClock()
    {
        Vector3[] positions = new Vector3[cubeCount];

        // 1) Define your 2D control points (normalized Y from –1 … +1).
        Vector2[] control2D = new Vector2[]
        {
        new Vector2( 0f,   1.00f),   // top tip (normalized)
        new Vector2( 0.15f, 0.80f),
        new Vector2( 0.25f, 0.50f),
        new Vector2( 0.10f, 0.30f),
        new Vector2( 0.25f, 0.10f),
        new Vector2( 0.15f,-0.10f),
        new Vector2( 0f,  -1.00f)    // bottom tip (normalized)
        };

        // 2) Scale those control points by formationRadius.
        for (int i = 0; i < control2D.Length; i++)
            control2D[i] *= formationRadius;

        // 3) Compute how many cubes go on each half (right vs. left)
        int halfCubes = cubeCount / 2;
        bool isOdd = (cubeCount % 2 == 1);
        int samplesOnOneSide = halfCubes;

        int idx = 0;
        int segments = control2D.Length - 1;
        float totalT = segments;

        // 4) Sample "right side" and mirror to "left side"
        //    but invert Z within [0 … R] by doing zRaw = R - zRaw.
        for (int i = 0; i < samplesOnOneSide; i++)
        {
            float u = i / (float)(samplesOnOneSide - 1);
            float t = u * totalT;

            int seg = Mathf.FloorToInt(t);
            float localT = t - seg;

            int i0 = Mathf.Clamp(seg - 1, 0, control2D.Length - 1);
            int i1 = Mathf.Clamp(seg, 0, control2D.Length - 1);
            int i2 = Mathf.Clamp(seg + 1, 0, control2D.Length - 1);
            int i3 = Mathf.Clamp(seg + 2, 0, control2D.Length - 1);

            Vector2 P0 = control2D[i0];
            Vector2 P1 = control2D[i1];
            Vector2 P2 = control2D[i2];
            Vector2 P3 = control2D[i3];

            // Catmull–Rom spline (tension = 0.5)
            float tt = localT * localT;
            float ttt = tt * localT;
            Vector2 point2D = 0.5f * (
                (2f * P1) +
                (-P0 + P2) * localT +
                (2f * P0 - 5f * P1 + 4f * P2 - P3) * tt +
                (-P0 + 3f * P1 - 3f * P2 + P3) * ttt
            );

            // 5) Shrink X/Z by 0.5 (half‐size), then map Z into [0…R], then invert it:
            float rawZ = point2D.y * 0.5f + (formationRadius * 0.5f); // now ∈ [0…R]
            float zRaw = zRaw = (point2D.y * 0.5f) + (formationRadius * 0.5f);              // flipped into [R…0]

            float xRaw = point2D.x * 0.5f;

            Vector3 rightPos = transform.position + new Vector3(xRaw, 0f, zRaw);
            Vector3 leftPos = transform.position + new Vector3(-xRaw, 0f, zRaw);

            if (idx < cubeCount) positions[idx++] = rightPos;
            if (idx < cubeCount) positions[idx++] = leftPos;
        }

        // 6) If odd number of cubes, place the extra cube at the very “inverted top”:
        if (isOdd && idx < cubeCount)
        {
            // Original top tip was at point2D.y = +R. 
            // rawZ_top = (R * 0.5) + (R * 0.5) = R, 
            // then zRaw = R - R = 0 → the very bottom of our [0…R] band.
            positions[idx++] = transform.position + new Vector3(0f, 0f, 0f);
        }

        // 7) Apply jitter if you like:
        return positions;

    }

    // Add this to your CubeFormationController class:
    // Add this to your CubeFormationController class:
    Vector3[] GenerateFantasyEye()
    {
        Vector3[] positions = new Vector3[cubeCount];
        int index = 0;

        float eyeWidth = formationRadius * 1.8f;
        float eyeHeight = formationRadius;
        float irisRadius = formationRadius * 0.6f;
        float pupilRadius = formationRadius * 0.2f;
        float flameRadius = formationRadius * 1.5f;

        // Shape parameters
        float eyeSharpness = 2.5f; // Higher = sharper cat-eye shape
        float eyelidCurve = 0.7f;
        int spiralArms = 5;

        // Distribute cubes
        int outlineCount = Mathf.Min((int)(cubeCount * 0.3f), cubeCount);
        int irisCount = Mathf.Min((int)(cubeCount * 0.4f), cubeCount - index);
        int pupilCount = Mathf.Min((int)(cubeCount * 0.1f), cubeCount - index - outlineCount);
        int flameCount = cubeCount - outlineCount - irisCount - pupilCount;

        // 1. Cat-Eye Outline (Superellipse equation)
        for (int i = 0; i < outlineCount; i++)
        {
            float angle = (float)i / outlineCount * Mathf.PI * 2f;
            float x = Mathf.Pow(Mathf.Abs(Mathf.Cos(angle)), 2f / eyeSharpness) * eyeWidth * Mathf.Sign(Mathf.Cos(angle));
            float z = Mathf.Pow(Mathf.Abs(Mathf.Sin(angle)), 2f / eyelidCurve) * eyeHeight * Mathf.Sign(Mathf.Sin(angle));
            positions[index++] = transform.position + new Vector3(x, 0, z);
        }

        // 2. Cosmic Iris (Spiral pattern with golden ratio)
        float goldenAngle = 137.508f * Mathf.Deg2Rad;
        for (int i = 0; i < irisCount; i++)
        {
            float radius = irisRadius * Mathf.Sqrt((float)i / irisCount);
            float angle = i * goldenAngle;

            // Add spiral arm modulation
            float armOffset = (i % spiralArms) * (Mathf.PI * 2f / spiralArms);
            float spiralDensity = 1f + Mathf.Sin(angle * 0.5f + armOffset) * 0.3f;

            positions[index++] = transform.position + new Vector3(
                Mathf.Cos(angle) * radius * spiralDensity,
                0,
                Mathf.Sin(angle) * radius * spiralDensity * 0.8f
            );
        }

        // 3. Eldritch Pupil (Hexagonal pattern with central glow)
        int rings = 3;
        int perRing = 6;
        for (int r = 0; r < rings && index < pupilCount + outlineCount + irisCount; r++)
        {
            float ringRadius = pupilRadius * (r + 1) / (float)rings;
            for (int a = 0; a < perRing * (r + 1); a++)
            {
                float angle = a * Mathf.PI * 2f / (perRing * (r + 1));
                positions[index++] = transform.position + new Vector3(
                    Mathf.Cos(angle) * ringRadius,
                    0,
                    Mathf.Sin(angle) * ringRadius * 0.6f
                );
                if (index >= pupilCount + outlineCount + irisCount) break;
            }
        }

        // 4. Arcane Flames (Procedural energy tendrils)
        int tendrils = 8;
        int cubesPerTendril = flameCount / tendrils;
        for (int t = 0; t < tendrils; t++)
        {
            float baseAngle = t * Mathf.PI * 2f / tendrils;
            float noiseOffset = Random.Range(0f, 10f);

            for (int c = 0; c < cubesPerTendril; c++)
            {
                float progress = (c + 1) / (float)cubesPerTendril;
                float angle = baseAngle + Mathf.Sin(progress * Mathf.PI * 4f + noiseOffset) * 0.5f;
                float radius = flameRadius * progress * (1f + Mathf.PerlinNoise(t, c * 0.1f) * 0.3f);

                positions[index++] = transform.position + new Vector3(
                    Mathf.Cos(angle) * radius,
                    0,
                    Mathf.Sin(angle) * radius * 0.7f
                );
            }
        }

        // 5. Floating Runes (Random arcane symbols)
        int remaining = cubeCount - index;
        for (int i = 0; i < remaining; i++)
        {
            float angle = Random.Range(0, Mathf.PI * 2f);
            float radius = flameRadius * 1.1f + Random.Range(-0.2f, 0.2f) * formationRadius;
            float yOffset = Mathf.Sin(Time.time * 2f + i) * 0.3f * formationRadius;

            positions[index++] = transform.position + new Vector3(
                Mathf.Cos(angle) * radius,
                yOffset,
                Mathf.Sin(angle) * radius
            );
        }

        return ApplyJitter(positions);
    }


    // ─────────────────────────────────────────────────────────────────────────
    // Utility: Compute centroid of any formation
    Vector3 CalculateFormationCenter(Vector3[] positions)
    {
        Vector3 sum = Vector3.zero;
        foreach (var pos in positions)
            sum += pos;
        return sum / positions.Length;
    }

    Vector3[] ApplyJitter(Vector3[] positions)
    {
        for (int i = 0; i < positions.Length; i++)
        {
            positions[i] += new Vector3(
                Random.Range(-jitterAmount, jitterAmount),
                Random.Range(-jitterAmount, jitterAmount),
                Random.Range(-jitterAmount / 3, jitterAmount / 3)
            );
        }
        return positions;
    }

    private void OnDisable()
    {
        FormationConsequenceManager.Unregister(this);
    }
}

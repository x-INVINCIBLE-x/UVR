using System;
using System.Collections;
using UnityEditor.Overlays;
using UnityEngine;

public class FormationHandler : MonoBehaviour
{
    public static FormationHandler Instance { get; private set; }
    [System.Serializable]
    public class FormationInfo
    {
        [HideInInspector] public string layoutName;
        public Transform layout;
        public Vector3 position;
        public Vector3 rotation;
        public float duration;
    }

    [System.Serializable]
    public class Formation
    {
#if UNITY_EDITOR
        public string formationName;
        public int formationNumber;
#endif
        public FormationInfo[] formationInfo;
    }

    [System.Serializable]
    public class ScalingFormation
    {
        public int difficultyLevel;
        public Formation[] formations;
    }

    [System.Serializable]
    public class TimedFormation
    {
        public DynamicFormationController cubeFormationController;
        public int duration;
    }

    public GridFormationController[] gridSpawners;
    [SerializeField] private int gridChangeDuration = 60;

    public TimedFormation[] timedForamtions;
    public ScalingFormation[] scalingFormations;

    private int index;
    private int currentDifficulty;

#if UNITY_EDITOR
    private void OnValidate()
    {
        for (int i = 0; i < scalingFormations.Length; i++)
        {
            scalingFormations[i].difficultyLevel = i + 1;

            for (int j = 0; j < scalingFormations[i].formations.Length; j++)
            {
                scalingFormations[i].formations[j].formationNumber = j + 1;

                for (int k = 0; k < scalingFormations[i].formations[j].formationInfo.Length; k++)
                {
                    scalingFormations[i].formations[j].formationInfo[k].layoutName = scalingFormations[i].formations[j].formationInfo[k].layout.name;
                }
            }
        }
    }
#endif

    //private void Awake()
    //{
    //    Debug.Log("finding layouts for formations");
    //    foreach (ScalingFormation scalingFormation in scalingFormations)
    //    {
    //        foreach (Formation formation in scalingFormation.formations)
    //        {
    //            foreach (FormationInfo info in formation.formationInfo)
    //            {
    //                if (!string.IsNullOrEmpty(info.layoutName))
    //                {
    //                    info.layout = null;
    //                    GameObject obj = FindChildByNameRecursive(transform.root, info.layoutName);
    //                    if (obj == null)
    //                    {
    //                        Debug.LogWarning($"Layout '{info.layoutName}' not found in the hierarchy. Please ensure it exists.");
    //                    }
    //                    else
    //                    {
    //                        Debug.Log($"Found layout: {obj.name}");
    //                        info.layout = obj.transform;
    //                    }                            
    //                }
    //            }
    //        }
    //    }
    //}

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }   
    }

    private void Start()
    {
        ChallengeManager challengeManager = ChallengeManager.instance;

        if (challengeManager != null)
        {
            ChallengeManager.instance.OnChallengeStart += HandleChallengeStart;
            ChallengeManager.instance.OnChallengeSuccess += HandleLevelEnd;
            ChallengeManager.instance.OnChallengeFail += HandleLevelEnd;
        }

        if (DungeonManager.Instance != null)
        {
            currentDifficulty = DungeonManager.Instance.DifficultyLevel - 1;

            if (currentDifficulty >= scalingFormations.Length)
            {
                Debug.Log("<color=cyan>  " + gameObject.name + "</color> <color=green> Does Not have INFO about this Difficulty Level. Reverting to LAST possible DIFFICULTY </color>");
                currentDifficulty = scalingFormations.Length - 1;
            }
        }

        timedForamtions[0].cubeFormationController.OnFormationComplete += UpdateFormation;

        //FormationActiveStatusTo(false);
        CloseAllTimedFormation();
    }

    private void HandleLevelEnd()
    {
        FormationActiveStatusTo(false);
        StopAllCoroutines();
    }

    private void HandleChallengeStart(ChallengeType type)
    {
        FormationActiveStatusTo(true);

        for (int i = 0; i < timedForamtions.Length; i++)
        {
            StartCoroutine(StartTimedFormationRoutine(timedForamtions[i].cubeFormationController, timedForamtions[i].duration));
        }

        if (type == ChallengeType.Slice)
        {
            GameEvents.OnElimination += HandleElimination;
        }
        else
        {
            for (int i = 0; i < gridSpawners.Length; i++)
            {
                Debug.Log("Grid Routine");
                StartCoroutine(StartTimedFormationRoutine(gridSpawners[i], gridChangeDuration));
            }
            
        }
    }

    private void HandleElimination(ObjectiveType type)
    {
        if (type == ObjectiveType.Crystal)
        {
            for (int i = 0; i < gridSpawners.Length; i++)
            {
                gridSpawners[i].NextTransition();
            }
        }
    }

    private void FormationActiveStatusTo(bool status)
    {
        foreach (Formation formation in scalingFormations[currentDifficulty].formations)
        {
            for (int j = 0; j < formation.formationInfo.Length; j++)
            {
                if (formation.formationInfo[j].layout == null)
                {
                    Debug.LogWarning($"Layout for formation '{formation.formationName}' is not set. Please check the setup.");
                    continue;
                }
                formation.formationInfo[j].layout.gameObject.SetActive(status);
            }
        }
    }

    private void CloseAllTimedFormation()
    {
        for (int i = 0; i < timedForamtions.Length; i++)
        {
            timedForamtions[i].cubeFormationController.gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        //if (Input.GetKeyDown(KeyCode.Escape))
        //{
        //    UpdateFormation();
        //}
    }

    public void UpdateFormation(FormationType type)
    {
        Formation nextFormation = scalingFormations[currentDifficulty].formations[index];
        index = (index + 1) % scalingFormations.Length;

        foreach (FormationInfo formationInfo in nextFormation.formationInfo)
        {
            StartCoroutine(MoveToTarget(formationInfo));
        }
    }

    private IEnumerator MoveToTarget(FormationInfo info)
    {
        Transform layout = info.layout;
        layout.GetPositionAndRotation(out Vector3 startPos, out Quaternion startRot);

        Vector3 endPos = info.position;
        Quaternion endRot = Quaternion.Euler(info.rotation);

        float elapsed = 0f;

        while (elapsed < info.duration)
        {
            float t = elapsed / info.duration;
            layout.SetPositionAndRotation(Vector3.Lerp(startPos, endPos, t), Quaternion.Slerp(startRot, endRot, t));
            elapsed += Time.deltaTime;
            yield return null;
        }

        layout.SetPositionAndRotation(endPos, endRot);
    }

    private IEnumerator StartTimedFormationRoutine(FormationProvider timedFormation, int duration)
    {
        while (true)
        {
            yield return new WaitForSeconds(duration);
            Debug.Log("Grid Routine Restarty");

            if (timedFormation.gameObject.activeSelf)
                timedFormation.NextTransition();
        }
    }

    private void OnDestroy()
    {
        ChallengeManager.instance.OnChallengeStart -= HandleChallengeStart;
        ChallengeManager.instance.OnChallengeSuccess -= HandleLevelEnd;
        ChallengeManager.instance.OnChallengeFail -= HandleLevelEnd;
        timedForamtions[0].cubeFormationController.OnFormationComplete -= UpdateFormation;
    }

    public GameObject FindChildByNameRecursive(Transform parent, string childName)
    {
        foreach (Transform child in parent)
        {
            if (child.name == childName)
                return child.gameObject;

            GameObject result = FindChildByNameRecursive(child, childName);
            if (result != null)
                return result;
        }
        return null;
    }
}

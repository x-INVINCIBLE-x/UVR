using System;
using System.Collections;
using UnityEngine;

public class FormationHandler : MonoBehaviour
{
    [System.Serializable]
    public class FormationInfo
    {
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
        public FormationProvider cubeFormationController;
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
            }
        }
    }
#endif

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

        //FormationActiveStatusTo(false);
        CloseAllTimedFormation();
    }

    private void HandleLevelEnd()
    {
        FormationActiveStatusTo(false);
        StopAllCoroutines();
    }

    private void HandleChallengeStart(string challengeNanme)
    {
        FormationActiveStatusTo(true);

        for (int i = 0; i < timedForamtions.Length; i++)
        {
            StartCoroutine(StartTimedFormationRoutine(timedForamtions[i].cubeFormationController, timedForamtions[i].duration));
        }

        if (challengeNanme == "Slice")
        {
            EnemyEvents.OnElimination += HandleElimination;
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
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            UpdateFormation();
        }
    }

    public void UpdateFormation()
    {
        if (index >= scalingFormations.Length)
        {
            Debug.Log("Reached End of Formations");
            return;
        }

        Formation nextFormation = scalingFormations[currentDifficulty].formations[index++];

        foreach (FormationInfo formationInfo in nextFormation.formationInfo)
        {
            StartCoroutine(MoveToTarget(formationInfo));
        }
    }

    private IEnumerator MoveToTarget(FormationInfo info)
    {
        Transform layout = info.layout;
        Vector3 startPos = layout.position;
        Quaternion startRot = layout.rotation;

        Vector3 endPos = info.position;
        Quaternion endRot = Quaternion.Euler(info.rotation);

        float elapsed = 0f;

        while (elapsed < info.duration)
        {
            float t = elapsed / info.duration;
            layout.position = Vector3.Lerp(startPos, endPos, t);
            layout.rotation = Quaternion.Slerp(startRot, endRot, t);

            elapsed += Time.deltaTime;
            yield return null;
        }

        layout.position = endPos;
        layout.rotation = endRot;
    }

    private IEnumerator StartTimedFormationRoutine(FormationProvider timedFormation, int duration)
    {
        while (true)
        {
            yield return new WaitForSeconds(duration);
            Debug.Log("Grid Routine Restarty");

            timedFormation.NextTransition();
        }
    }
}

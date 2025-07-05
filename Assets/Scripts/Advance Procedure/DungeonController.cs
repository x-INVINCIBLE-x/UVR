using System.Collections;
using UnityEngine;

public class DungeonController : MonoBehaviour
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
#endif
        public FormationInfo[] formaationInfo;
    }

    [System.Serializable]
    public class TimedFormation
    {
        public CubeFormationController cubeFormationController;
        public float duration;
    }

    public GridCuboidSpawner[] gridSpawners;
    public TimedFormation[] timedForamtions;
    public Formation[] formations;

    private int index;

    private void Start()
    {
        
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
        if (index >= formations.Length)
        {
            Debug.Log("Reached End of Formations");
            return;
        }

        Formation nextFormation = formations[index++];

        foreach (FormationInfo formationInfo in nextFormation.formaationInfo)
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

    private IEnumerator StartTimedFormation(TimedFormation timedFormation)
    {
        timedFormation.cubeFormationController.NextTransition();
        yield return new WaitForSeconds(timedFormation.duration);
    }
}

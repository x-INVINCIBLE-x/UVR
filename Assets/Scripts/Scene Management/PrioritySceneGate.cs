using UnityEngine;

public class PrioritySceneGate : MonoBehaviour
{
    public static PrioritySceneGate Instance;

    public bool IsReady => ready;
    private bool ready = false;

    private void Awake()
    {
        Instance = this;
    }

    public void MarkReady()
    {
        ready = true;
    }
}
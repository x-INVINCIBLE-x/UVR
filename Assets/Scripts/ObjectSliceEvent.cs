using UnityEngine;

public class ObjectSliceEvent : MonoBehaviour,ISliceable
{
    [SerializeField] private ObjectiveType type;
    private bool hasRaisedEvent;

    private void Awake()
    {
        hasRaisedEvent = false;
    }


    public void HandleSlice()
    {   
        if(hasRaisedEvent == false)
        {
            hasRaisedEvent = true;
            GameEvents.OnElimination?.Invoke(type);
        }
    }
}

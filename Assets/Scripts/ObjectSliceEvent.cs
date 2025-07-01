using UnityEngine;

public class ObjectSliceEvent : MonoBehaviour,ISliceEvent
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
            EnemyEvents.OnElimination?.Invoke(type);
        }
    }
}

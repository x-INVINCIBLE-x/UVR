using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SliceRewardable :MonoBehaviour,ISliceable, IRewardProvider<GameReward>
{
    [SerializeField] private GameReward sliceRewards;
    [SerializeField] private ObjectDissolver dissolver;
    [SerializeField] private string AfterSliceLayer = "Sliced";// sliced layer for dissolving
    private bool isDissolving = false;
    
    private void Start()
    {
        dissolver = GetComponent<ObjectDissolver>();
    }

    private void Update()
    {
        if (!isDissolving)
        {
            int targetLayerInt = LayerMask.NameToLayer(AfterSliceLayer); // Layer set up
            if (gameObject.layer == targetLayerInt)
            {
                Debug.Log("Dissolver Start");
                dissolver.Dissolve = true;
                isDissolving = true;
                Destroy(gameObject, 2f);
            }
        }
           
    }

    public GameReward GetReward()
    {
        return sliceRewards;
    }

    public void HandleSlice()
    {
        GameEvents.RaiseReward(this);

    }
}

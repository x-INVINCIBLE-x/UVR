using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Locomotion;

public class SliceRewardable :MonoBehaviour,ISliceable, IRewardProvider<GameReward>
{
    [SerializeField] private GameReward sliceRewards;
    [SerializeField] private ObjectDissolver dissolver;
    [SerializeField] private ObjectDissolver[] childDissolvers;
    [SerializeField] private string AfterSliceLayer = "Sliced";// sliced layer for dissolving
    private bool isDissolving = false;

    [ContextMenu("Get Child Dissolvers")]
    private void GetChildDissolvers()
    {
        dissolver = GetComponent<ObjectDissolver>();
        childDissolvers = GetComponentsInChildren<ObjectDissolver>();
    }

    public GameReward GetReward()
    {
        return sliceRewards;
    }

    public void HandleSlice()
    {
        GameEvents.RaiseReward(this);

        foreach (var disolver in childDissolvers)
        {
            if (disolver != null)
            {
                if (!disolver.TryGetComponent(out Rigidbody _))
                {
                    Rigidbody rb = disolver.AddComponent<Rigidbody>();
                    rb.mass = 30f;
                    Destroy(disolver.gameObject,3f);
                }


                disolver.Dissolve = true;
            }
        }
    }

    public void HandleInstanceSlice()
    {
        if (isDissolving) return;
        int targetLayerInt = LayerMask.NameToLayer(AfterSliceLayer); // Layer set up
        dissolver = GetComponentInChildren<ObjectDissolver>();
        if (gameObject.layer == targetLayerInt)
        {
            dissolver.Dissolve = true;
            isDissolving = true;
            Destroy(gameObject, 2f);
        }
    }

    public void PreSlice()
    {
        foreach (var disolver in childDissolvers)
        {
            if (disolver != null)
            {
                disolver.transform.parent = null;
            }
        }
    }
}

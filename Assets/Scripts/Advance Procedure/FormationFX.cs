using System;
using UnityEngine;

public class FormationFX : MonoBehaviour
{
    [SerializeField] private CubeFormationController cubeFormationController;

    private void Awake()
    {
        cubeFormationController.OnFormationStart += HandleFormationStart;
        cubeFormationController.OnFormationComplete += HandleFormationComplete;
        cubeFormationController.OnUnwrapStart += HandleUnwrapStart;
        cubeFormationController.OnUnwrapComplete += HandleUnwrapComplete;
    }

    private void HandleUnwrapComplete()
    {
        Debug.Log("Unwrap Complete");
    }

    private void HandleUnwrapStart()
    {
        Debug.Log("Unwrap Start");
    }

    private void HandleFormationComplete(FormationType formationType)
    {
        Debug.Log("Formation Complete -> " + formationType);
    }

    private void HandleFormationStart()
    {
        Debug.Log("Formation Start");
    }
}

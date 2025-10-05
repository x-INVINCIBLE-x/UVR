using UnityEngine;

public class FormationFX : MonoBehaviour
{
    [SerializeField] private DynamicFormationController cubeFormationController;
    [SerializeField] private AudioClip formationCompleteAudio;
    private void Start()
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

    private void HandleUnwrapStart(FormationType formationType)
    {
        Debug.Log("Unwrap Start");
    }

    private void HandleFormationComplete(FormationType formationType)
    {
        Debug.Log("Formation Complete -> " + formationType);
        AudioManager.Instance.PlaySystemSFX(formationCompleteAudio);
    }

    private void HandleFormationStart()
    {
        Debug.Log("Formation Start");
    }

    private void OnDestroy()
    {
        cubeFormationController.OnFormationStart -= HandleFormationStart;
        cubeFormationController.OnFormationComplete -= HandleFormationComplete;
        cubeFormationController.OnUnwrapStart -= HandleUnwrapStart;
        cubeFormationController.OnUnwrapComplete -= HandleUnwrapComplete;
    }
}

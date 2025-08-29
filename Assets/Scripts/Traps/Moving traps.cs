using DG.Tweening;
using UnityEngine;

public class Movingtraps : MonoBehaviour
{
    [Space]
    [Header("Moving trap Setup")]
    [Space]
    public Vector3 movetoOffset;
    public Vector3 rotationSetting;
    private Vector3 startPosition;
    [SerializeField] private float moveDuration;
    

    [Space]
    [Header("Ease Type")]
    [Space]
    [SerializeField] Ease ease; // ease type 

    [Space]
    [Header("loopType")]
    [Space]
    [SerializeField] LoopType loopType; // Looping type


    private void Start()
    {
        startPosition = transform.localPosition;
        MovingTrapActivate();
    }

    public void MovingTrapActivate()
    {
        //Debug.Log("Moving Trap Activated");
        // Add logic for Moving traps
        Debug.Log($"Moving Trap Activated with ease: {ease}");
        transform.DOLocalMove(startPosition + movetoOffset, moveDuration).SetEase(ease).SetLoops(-1, loopType);
        RotateTrap();

    }

    public void RotateTrap()
    {
        transform.DOLocalRotate(rotationSetting, 1.5f, RotateMode.FastBeyond360).SetLoops(-1, LoopType.Restart).SetRelative().SetEase(Ease.Linear);
    }
}

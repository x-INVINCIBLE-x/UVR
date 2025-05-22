using DG.Tweening;
using UnityEngine;

public class FlyingShipEnemy : MonoBehaviour
{
    private Vector3 forwardOffset;
    private Vector3 backwardOffset;
    private Vector3 leftOffset;
    private Vector3 rightOffset;
    private Vector3[] waypoints;

    [SerializeField] private float offsetMultiplier = 1f;
    [SerializeField] private float duration = 1f;
    [SerializeField] private int resolution =1;


    private void Awake()
    {
        forwardOffset = transform.position + new Vector3(0f,0f,1f * offsetMultiplier);
        backwardOffset = transform.position + new Vector3(0f,0f,-1f * offsetMultiplier);
        leftOffset = transform.position + new Vector3(-1f * offsetMultiplier, 0f,0f);
        rightOffset = transform.position + new Vector3(1f * offsetMultiplier, 0f,0f);
        waypoints = new[] { forwardOffset , leftOffset , backwardOffset , rightOffset , forwardOffset};
    }

    private void Start()
    {
        
        transform.DOPath(waypoints, duration, PathType.CatmullRom, PathMode.Full3D, resolution, Color.green).SetLoops(-1,LoopType.Incremental).SetLookAt(0.1f);
    }

    private void Update()
    {
        
    }
}

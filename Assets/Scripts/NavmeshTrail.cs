using System;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class NavmeshTrail : MonoBehaviour
{
    [Header("Trail Settings")]
    public Transform Destination;
    [SerializeField] private GameObject trailVFX;
    [SerializeField] private float trailSpeed = 2f;
    [SerializeField] private float reachThreshold = 0.2f; // how close is "reached"

    private NavMeshAgent trailAgent;
    private Vector3 playerPosition;

    private void Start()
    {
        InputManager.Instance.XHold.action.performed += ActivateTrail;
        trailAgent = GetComponent<NavMeshAgent>();
        trailAgent.speed = trailSpeed;
        trailVFX.SetActive(false);
        playerPosition = PlayerManager.instance.PlayerOrigin.transform.position;
        gameObject.transform.position = playerPosition;
    }

    public void ActivateTrail(UnityEngine.InputSystem.InputAction.CallbackContext context)
    {
        Debug.Log("Activate");
        if (Destination != null)
        {   
            trailVFX.SetActive(true);
            playerPosition = PlayerManager.instance.PlayerOrigin.transform.position;
            gameObject.transform.position = playerPosition;
            SetTrailDestination(Destination.position);
        }
    }

    private void SetTrailDestination(Vector3 destination)
    {
        if (trailAgent.isOnNavMesh)
        {
            trailAgent.SetDestination(destination);
        }
    }

    private void Update()
    {
        if (trailAgent.hasPath && hasReachedDestination())
        {
            ResetTrail();
        }
    }

    private void ResetTrail()
    {
        trailVFX.SetActive(false);
        if (trailAgent.isOnNavMesh)
        {
            playerPosition = PlayerManager.instance.PlayerOrigin.transform.position;
            trailAgent.Warp(playerPosition); // reset to start
        }
    }

    private bool hasReachedDestination()
    {
        if (!trailAgent.pathPending && trailAgent.remainingDistance <= trailAgent.stoppingDistance + reachThreshold)
        {
            return true;
        }
        return false;
    }

 
}


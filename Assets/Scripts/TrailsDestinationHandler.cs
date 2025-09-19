using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class TrailsDestinationHandler : MonoBehaviour
{
    [SerializeField] GameObject Trail;
    public List<GameObject> Waypoints;
    [SerializeField] float timetoReachDestination = 10f;
    [SerializeField] int resoultion = 10;
    private List<Vector3> WaypointsVector3;
    private Vector3 originalTrailPosition;

    private void Awake()
    {
        // Store original position
        originalTrailPosition = Trail.transform.position;

        // Initialize the list
        WaypointsVector3 = new List<Vector3>();

        // Add each waypoint's position to the Vector3 list
        foreach (GameObject waypoint in Waypoints)
        {   
            WaypointsVector3.Add(waypoint.transform.position);
        }

        // Initially disable the trail
        Trail.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<TerrainScanner2>() == null) return;

        // Start following waypoints
        FollowWaypoints();
    }

    private void FollowWaypoints()
    {
        if (WaypointsVector3 == null || WaypointsVector3.Count == 0)
        {
            Debug.LogWarning("No waypoints available!");
            return;
        }

        // Enable the trail
        Trail.SetActive(true);

        // Create the path animation
        Trail.transform.DOPath(WaypointsVector3.ToArray(), timetoReachDestination, PathType.CubicBezier, PathMode.Full3D, resoultion)
            .OnComplete(() =>
            {
                // When path is complete, reset position and disable
                ResetTrail();
            });
    }

    private void ResetTrail()
    {
        // Reset to original position
        Trail.transform.position = originalTrailPosition;

        // Disable the trail
        Trail.SetActive(false);
    }

    private void OnDisable()
    {
        ResetTrail();
    }
}
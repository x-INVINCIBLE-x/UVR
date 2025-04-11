using System.Collections.Generic;
using UnityEngine;

public enum RingAxis
{
    XY,
    XZ,
    YZ
}

public class RingUIHelper : MonoBehaviour
{
    public List<Transform> slots = new();
    public float radius = 2f;
    public float heightOffset = 1.5f;
    public float lookOffest = 0f;
    [Range(0, 360)]
    public int angleToUse = 360;
    public int startOffset = 0;
    public RingAxis ringAxis = RingAxis.XZ;

    private void OnValidate()
    {
        PositionSlots();
    }

    void Update()
    {
        PositionSlots();
    }

    void PositionSlots()
    {
        //slots = GetComponentsInChildren<Transform>().ToList();
        slots.Remove(transform);

        if (slots.Count == 0)
        {
            return;
        }

        float angleStep = angleToUse / slots.Count + startOffset;

        for (int i = 0; i < slots.Count; i++)
        {
            float angle = angleStep * (i + 1);

            Vector3 worldPos = Vector3.zero;
            if (ringAxis == RingAxis.XZ)
            {
                worldPos = PositionInXZ(angle);
            }
            else if (ringAxis == RingAxis.XY)
            {
                worldPos = PositionInXY(angle);
            }
            else
            {
                worldPos = PositionInYZ(angle);
            }
            slots[i].position = worldPos;

            slots[i].LookAt(transform.position + new Vector3(0, lookOffest, 0));
        }
    }
    private Vector3 PositionInXY(float angle)
    {
        return transform.position + new Vector3(Mathf.Cos(angle * Mathf.Deg2Rad) * radius, Mathf.Sin(angle * Mathf.Deg2Rad) * radius, heightOffset);
    }

    private Vector3 PositionInXZ(float angle)
    {
        return transform.position + new Vector3(Mathf.Cos(angle * Mathf.Deg2Rad) * radius, heightOffset, Mathf.Sin(angle * Mathf.Deg2Rad) * radius);
    }

    private Vector3 PositionInYZ(float angle)
    {
        return transform.position + new Vector3(heightOffset, Mathf.Cos(angle * Mathf.Deg2Rad) * radius, Mathf.Sin(angle * Mathf.Deg2Rad) * radius);
    }
}

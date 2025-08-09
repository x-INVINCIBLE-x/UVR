using UnityEngine;

public class ParabolicBullet : MonoBehaviour
{
    public Transform target;

    public float goUpSpeed = 10f;     // Speed of ascent
    public float fallSpeed = 15f;     // Speed of descent
    public float arcHeight = 5f;
    public float chaseDuration = 1.0f;
    public bool faceMovement = true;

    private Vector3 startPoint;
    private Vector3 peakPoint;
    private Vector3 finalTargetPosition;

    private bool isFalling = false;
    private bool isChasing = true;
    private bool started = false;

    void Start()
    {
        StartParabola();
    }

    public void StartParabola()
    {
        if(target == null) return;

        started = true;
        startPoint = transform.position;
        finalTargetPosition = target.position;
        UpdateControlPoint();
    }

    void Update()
    {
        if (!started) return;

        if (isChasing)
        {
            chaseDuration -= Time.deltaTime;

            if (chaseDuration > 0f)
            {
                finalTargetPosition = target.position;
                UpdateControlPoint();
            }
            else
            {
                isChasing = false;
                //finalTargetPosition = finalTargetPosition; // lock final target
            }
        }

        if (!isFalling)
        {
            Vector3 direction = (peakPoint - transform.position).normalized;
            transform.position += direction * goUpSpeed * Time.deltaTime;

            if (faceMovement && direction != Vector3.zero)
                transform.rotation = Quaternion.LookRotation(direction);

            // If reached or passed the peak
            if (Vector3.Dot(peakPoint - transform.position, direction) <= 0f)
            {
                transform.position = peakPoint;
                isFalling = true;
            }
        }
        else
        {
            Vector3 direction = (finalTargetPosition - transform.position).normalized;
            transform.position += direction * fallSpeed * Time.deltaTime;

            if (faceMovement && direction != Vector3.zero)
                transform.rotation = Quaternion.LookRotation(direction);
        }
    }

    void UpdateControlPoint()
    {
        Vector3 mid = (startPoint + finalTargetPosition) * 0.5f;
        peakPoint = mid + Vector3.up * arcHeight;
    }
}

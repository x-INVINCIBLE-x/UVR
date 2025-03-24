using UnityEngine;

public class SwipeRotate : MonoBehaviour
{
    [SerializeField] private GameObject objectToRotate;
    [SerializeField] private float maxSpeed = 2f;
    [SerializeField] private float acceleration = 20f;
    private Transform handTransform;
    private Vector3 startPosition;
    private bool isActive = false;

    private void Update()
    {
        if (!isActive || objectToRotate == null || handTransform == null)
        {
            return;
        }

        float distance = Mathf.Abs(startPosition.x - handTransform.position.x);
        float speed = Mathf.Clamp(distance * acceleration, 0f, maxSpeed);

        int dir = handTransform.position.x > startPosition.x ? 1 : -1;
        objectToRotate.transform.Rotate(Vector3.forward, dir * speed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Enter");
        isActive = true;
        handTransform = other.transform;
        startPosition = other.transform.position;
    }

    private void OnTriggerExit(Collider other)
    {
        Debug.Log("Exit");
        isActive = false;
        handTransform = null;
        startPosition = Vector3.zero;
    }
}

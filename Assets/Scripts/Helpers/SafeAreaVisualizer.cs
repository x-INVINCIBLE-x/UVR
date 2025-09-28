using UnityEngine;

public class SafeAreaVisualizer : MonoBehaviour
{
    [SerializeField] private Vector2 safeArea;
    [SerializeField] private Vector3 positionInfo;
    [SerializeField] private Vector3 rotationInfo;

    private void OnDrawGizmosSelected()
    {
        positionInfo = transform.position;
        rotationInfo = transform.eulerAngles;

        Gizmos.color = Color.red;

        Matrix4x4 oldMatrix = Gizmos.matrix;

        Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, Vector3.one);

        Gizmos.DrawWireCube(Vector3.zero, new Vector3(safeArea.x, 0, safeArea.y));

        Gizmos.matrix = oldMatrix;
    }
}

using UnityEngine;

public class RemoveParent : MonoBehaviour
{
    void Start()
    {
        Invoke(nameof(Remove), 1f);
    }

    private void Remove()
    {
        transform.parent = null;

    }
}

using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager instance;
    [field: SerializeField] public Transform player {  get; private set; }

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }
}

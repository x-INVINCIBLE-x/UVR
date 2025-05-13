using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager instance;
    [field: SerializeField] public Player player {  get; private set; }
    [field: SerializeField] public Rigidbody rb { get; private set; }

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }
}

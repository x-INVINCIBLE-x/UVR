using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager instance;
    [field: SerializeField] public Player player {  get; private set; }
    [field: SerializeField] public Rigidbody rb { get; private set; }

    public event System.Action OnPlayerDeath
    {
        add => player.stats.OnPlayerDeath += value;
        remove => player.stats.OnPlayerDeath -= value;
    }

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }
}

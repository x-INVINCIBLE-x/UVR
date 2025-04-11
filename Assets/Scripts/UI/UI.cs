using UnityEngine;

public class UI : MonoBehaviour
{
    public static UI Instance;
    public PlayerUI playerUI { get; private set; }

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        playerUI = GetComponent<PlayerUI>();
    }
}

using UnityEngine;



public class DungeonMusic : MonoBehaviour
{   
    public AudioClip dungeonTheme;
    public AudioClip battleTheme;

    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        AudioManager.Instance.PlayMusic(dungeonTheme);   
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            AudioManager.Instance.PlayMusicWithCrossFade(battleTheme,1f);
        }
    }
}

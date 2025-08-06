using UnityEngine;

[CreateAssetMenu(menuName = "Challenge/Challenge UI Data")]
public class ChallengeUIData : ScriptableObject
{
    public string challengeID;
    public string displayName;
    [TextArea] public string description;
    [TextArea] public string flavourText;
    public Sprite icon;
    public GameObject startStatue; 
}
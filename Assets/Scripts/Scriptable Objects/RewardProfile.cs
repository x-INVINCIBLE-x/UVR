using UnityEngine;

[CreateAssetMenu(fileName = "New Reward Profile", menuName = "Game/Reward Profile")]
public class RewardProfile : ScriptableObject
{
    [SerializeField] private int gold = 10;
    [SerializeField] private int magika = 10;
    public float currencyMultiplier = 1f;

    public int GetGold() => Mathf.RoundToInt(gold * currencyMultiplier);
    public int GetMagika() => Mathf.RoundToInt(magika * currencyMultiplier);
}
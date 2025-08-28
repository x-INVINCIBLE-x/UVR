using UnityEngine;

[CreateAssetMenu(fileName = "Currency_Effect", menuName = "Effects/Currency")]
public class CurrencyEffect : Effect, IRewardProvider<GameReward>
{
    [SerializeField] private int goldAmount;
    [SerializeField] private int magikaAmount;

    override public void Apply()
    {
        base.Apply();

        GameEvents.RaiseReward(this);
    }

    public GameReward GetReward()
    {
        return new GameReward(goldAmount, magikaAmount, 0);
    }

    override public void Remove()
    {
        base.Remove();
    }
}
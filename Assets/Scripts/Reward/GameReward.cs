[System.Serializable]
public class GameReward
{
    public int Gold;
    public int Magika;
    public int Experience;

    public GameReward(int gold, int magika, int experience)
    {
        Gold = gold;
        Magika = magika;
        Experience = experience;
    }
}

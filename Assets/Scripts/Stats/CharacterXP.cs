using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;

public class CharacterXP : MonoBehaviour
{
    [System.Serializable]
    public class LevelStep
    {
        public int startLevel;
        public int expIncrease;
    }

    [SerializeField] private int baseExp = 100;
    [SerializeField] private float growthFactor = 1.2f;

    [SerializeField] private List<LevelStep> expSteps = new List<LevelStep>();

    public int Level { get; private set; } = 1;
    public float CurrentExp { get; private set; } = 0f;

    public event Action<int> OnLevelUp;
    public event Action<float, float> OnExpChanged;

    public float ExpToNextLevel
    {
        get
        {
            int currentLevelExp = GetExpForLevel(Level);
            int nextLevelExp = GetExpForLevel(Level + 1);
            return nextLevelExp - currentLevelExp;
        }
    }

#if UNITY_EDITOR
    [Space]
    [SerializeField] private int expToDisplay = 10;
    [SerializeField] private int[] expRequird;

    private void OnValidate()
    {
        expRequird = new int[expToDisplay];
        for (int i = 0; i < expToDisplay; i++)
        {
            if (i == 0)
                expRequird[i] = baseExp;
            else
                expRequird[i] = Mathf.RoundToInt(expRequird[i - 1] * growthFactor);
        }
    }
#endif

    private void Start()
    {
        GameEvents.OnRewardProvided += HandleRewardProvided;
    }

    private void HandleRewardProvided(IRewardProvider<GameReward> provider)
    {
        AddExp(provider.GetReward().Experience);
    }

    public void AddExp(float amount)
    {
        CurrentExp += amount;
        OnExpChanged?.Invoke(CurrentExp, ExpToNextLevel);

        while (CurrentExp >= ExpToNextLevel)
        {
            LevelUp();
            OnExpChanged?.Invoke(CurrentExp, ExpToNextLevel);
        }
    }

    private void LevelUp()
    {
        CurrentExp -= ExpToNextLevel;
        Level++;
        OnLevelUp?.Invoke(Level);
    }

    public int GetExpForLevel(int level)
    {
        if (level <= 1)
            return baseExp;

        int exp = baseExp;

        for (int i = 2; i <= level; i++)
        {
            exp += GetIncreaseForLevel(i);
        }

        return exp;
    }

    private int GetIncreaseForLevel(int level)
    {
        int increase = expSteps.Count > 0 ? expSteps[0].expIncrease : 0;
        for (int i = 0; i < expSteps.Count; i++)
        {
            if (level >= expSteps[i].startLevel)
                increase = expSteps[i].expIncrease;
            else
                break;
        }
        return increase;
    }

    private int GetNextStepLevel(LevelStep currentStep)
    {
        int index = expSteps.IndexOf(currentStep);
        if (index + 1 < expSteps.Count)
            return expSteps[index + 1].startLevel;
        return int.MaxValue;
    }

    private void OnDestroy()
    {
        GameEvents.OnRewardProvided -= HandleRewardProvided;
    }
}
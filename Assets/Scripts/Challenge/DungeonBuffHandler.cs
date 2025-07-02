using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum BuffCategory
{
    Offense,
    Defence,
    Special
}

public class DungeonBuffHandler : MonoBehaviour
{
    [SerializeField] private BuffMaterialInfo[] materialsInfo;

    [field: SerializeField] public List<DungeonBuffProvider> buffProviders { get; private set; }
    public readonly float Dissolve_Duration = 4f;
    private Dictionary<BuffCategory, BuffMaterialInfo> materialInfoDic = new();

    public event System.Action OnBuffPick;

    private void Awake()
    {
        foreach (var provider in buffProviders)
        {
            provider.gameObject.SetActive(false);
        }
        
        foreach (BuffMaterialInfo info in materialsInfo)
        {
            materialInfoDic[info.Category] = info;
        }
    }

    public void ProvideBuffs(int index, Buff buffToProvide, BuffCategory buffCategory)
    {
        buffProviders[index].Initialize(this, buffToProvide, materialInfoDic[buffCategory]);
        buffProviders[index].gameObject.SetActive(true);
    }

    public void BuffPicked()
    {
        foreach (DungeonBuffProvider buffProvider in buffProviders)
        {
            buffProvider.Close();
        }
        buffProviders.Clear();

        OnBuffPick?.Invoke();
    }

    public void AddBuffProvider(DungeonBuffProvider buffProvider) => buffProviders.Add(buffProvider);
}

[System.Serializable]
public class BuffMaterialInfo
{
    public BuffCategory Category;
    public Material baseMaterial;
    public Material dissolveMaterial;
}

using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Challenge/Challenge UI Registry")]
public class ChallengeUIDataRegistry : ScriptableObject
{
    public List<ChallengeUIData> challengeUIDatas;

    private Dictionary<string, ChallengeUIData> lookup;

    public void Init()
    {
        lookup = new Dictionary<string, ChallengeUIData>();
        foreach (var data in challengeUIDatas)
        {
            if (!lookup.ContainsKey(data.challengeID))
                lookup.Add(data.challengeID, data);
        }
    }

    public ChallengeUIData GetUIData(string id)
    {
        if (lookup == null) Init();
        return lookup.TryGetValue(id, out var data) ? data : null;
    }
}
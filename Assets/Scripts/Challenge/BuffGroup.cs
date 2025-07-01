using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "BuffGroup By Difficulty")]
public class BuffGroup : ScriptableObject
{
    public int difficultyLevel;
    public List<BuffCategoryBind> buffsBind;
}

[System.Serializable]   
public class BuffCategoryBind
{
    public BuffCategory category;
    public List<Buff> buffs;
}
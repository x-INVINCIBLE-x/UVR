using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class ItemDatabase
{
    private static Dictionary<string, ItemData> items;

    static ItemDatabase()
    {
        items = Resources.LoadAll<ItemData>("").ToDictionary(i => i.ID, i => i);
    }

    public static ItemData GetItemByID(string id)
    {
        items.TryGetValue(id, out var itemData);
        return itemData;
    }
}

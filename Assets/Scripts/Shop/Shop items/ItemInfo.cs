using UnityEngine;

public enum ItemType
{
    Weapon,
    Consumable,
    Magic,
    Armor,
    Misc
}

[CreateAssetMenu(fileName = "NewItem", menuName = "Item System/Item Info")]
public class ItemInfo : ScriptableObject
{
    [field: SerializeField] public string ItemName { get; private set; }
    [field: SerializeField] public int ItemCost { get; private set; }
    [field: SerializeField] public ItemType ItemType { get; private set; }
    [field: SerializeField] public GameObject ItemPrefab { get; private set; }

    // Optional description, icon, rarity, etc.
    [field: SerializeField] public string Description { get; private set; }
    //[field: SerializeField] public Sprite Icon { get; private set; }
}

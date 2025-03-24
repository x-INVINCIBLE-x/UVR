using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[CreateAssetMenu(fileName = "InventoryItem", menuName = "Inventory/Inventory Item")]
public class InventoryItem
{
    public ItemData data;
    public int stackSize { get; private set; } = 0;
    public int stackLimit = 6;

    public InventoryItem(ItemData data)
    {
        this.data = data;
        stackLimit = data.StackLimit;
        stackSize = 1;
    }

    public bool AddToStack(int amount = 1)
    {
        if (stackSize == stackLimit)
        {
            return false;
        }

        stackSize += amount;
        return true;
    }

    public int RemoveFromStack(int amount = 1)
    {
        stackSize = Mathf.Max(0, stackSize - amount);
        return stackSize;
    }
}

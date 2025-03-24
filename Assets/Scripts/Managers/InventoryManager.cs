using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using System.Linq;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance {  get; private set; }
    public Dictionary<XRBaseInteractor, InventoryItem> weapondDict = new();

    public Dictionary<ItemData, ItemInfo> inventoryItemsDict = new();
    public UIItemSlot[] slots;
    public int busySlots = 0;

    //public Transform itemSlotParent;

    public class ItemInfo
    {
        public InventoryItem inventoryitem;
        public UIItemSlot slot;

        public ItemInfo(InventoryItem inventoryitem, UIItemSlot slot)
        {
            this.inventoryitem = inventoryitem;
            this.slot = slot;
        }
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        //slots = itemSlotParent.GetComponentsInChildren<UIItemSlot>();
    }

    public void AddItemFromSocket(InventoryItem item, XRBaseInteractor socket)
    {
        weapondDict.Add(socket, item);
    }

    public void RemoveItemFromSocket(XRBaseInteractor socket)
    {
        if (!weapondDict.ContainsKey(socket))
        {
            Debug.Log("Item to remove is not in Inventory");
            return;
        }

        weapondDict.Remove(socket);
    }

    public InventoryItem GetItem(XRBaseInteractor item)
    {
        if (!weapondDict.ContainsKey(item))
        {
            Debug.Log("Socket with no item requesting Inventory Item");
            return null;
        }

        return weapondDict[item];
    }

    public InventoryItem GetItem(ItemData data)
    {
        if (inventoryItemsDict.ContainsKey(data))
        {
            return inventoryItemsDict[data].inventoryitem;
        }

        return null;
    }

    public bool Additem(ItemData item)
    {
        if (item == null)
        {
            Debug.LogError("No item Data Attatched to the Inventory Item");
            return false;
        }

        if (inventoryItemsDict.ContainsKey(item))
        {
            bool isSuccess = inventoryItemsDict[item].inventoryitem.AddToStack();

            if (!isSuccess)
            {
                return false;
            }

            inventoryItemsDict[item].slot.UpdateCount(inventoryItemsDict[item].inventoryitem.stackSize);
        }
        else
        {
            if (busySlots >= slots.Length)
            {
                return false;
            }

            busySlots++;
            inventoryItemsDict.Add(item, new ItemInfo(new InventoryItem(item), null));
            AssignSlot(item);
        }

        Debug.Log(item.name + "->" + inventoryItemsDict.Count);
        return true;
    }

    public void RemoveItem(ItemData item)
    {
        if (!inventoryItemsDict.ContainsKey(item))
        {
            Debug.Log("Unknown Item asked to get removed");
            return;
        }

        int itemCount = inventoryItemsDict[item].inventoryitem.RemoveFromStack();
        inventoryItemsDict[item].slot.UpdateCount(itemCount);

        if (itemCount == 0)
        {
            inventoryItemsDict[item].slot.RemoveItem();
            inventoryItemsDict.Remove(item);
            busySlots--;
        }
    }

    public InventoryItem GetInventoryItem(ItemData item)
    {
        if (inventoryItemsDict.ContainsKey(item))
        {
            return inventoryItemsDict[item].inventoryitem;
        }

        return null;
    }

    private UIItemSlot AssignSlot(ItemData item)
    {
        foreach (var itemSlot in slots.Where(itemSlot => itemSlot.item == null))
        {
            itemSlot.AddItem(item, inventoryItemsDict[item].inventoryitem.stackSize);
            inventoryItemsDict[item].slot = itemSlot;
            return itemSlot;
        }

        return null;
    }

}
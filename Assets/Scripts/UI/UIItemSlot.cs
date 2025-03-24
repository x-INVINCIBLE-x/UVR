using JetBrains.Annotations;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIItemSlot : MonoBehaviour
{
    public ItemData item = null;
    [SerializeField] private Image image;
    [SerializeField] private Sprite defaultSprite;
    [SerializeField] private List<Transform> countDisplay;

    [SerializeField] private Transform countDisplayParent;
    public ItemsHandler inventoryHandler;

    private void Awake()
    {
        inventoryHandler = GetComponentInParent<ItemsHandler>();
    }

    private void Start()
    {
        countDisplay = countDisplayParent.GetComponentsInChildren<Transform>().ToList();
        countDisplay.Remove(countDisplayParent);
        RemoveItem();
    }

    public void AddItem(ItemData item, int amount)
    {
        this.item = item;

        image.sprite = item.Icon;

        UpdateCount(amount);
    }

    public void RemoveItem()
    {
        item = null;
        image.sprite = defaultSprite;

        foreach (Transform display in countDisplay)
        {
            display.gameObject.SetActive(false);
        }
    }

    public void UpdateCount(int amount)
    {
        for (int i = 0; i < countDisplay.Count; i++)
        {
            bool isActive = i < amount;
            countDisplay[i].gameObject.SetActive(isActive);
        }
    }

    public void Display()
    {
        if (item == null)
        {
            return;
        }

        inventoryHandler.DisplayItem(item);
    }

}

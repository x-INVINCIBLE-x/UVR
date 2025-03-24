using UnityEngine;

[CreateAssetMenu(fileName = "New Item Data", menuName = "Inventory/Item Data")]
public class ItemData : ScriptableObject
{
    [field: SerializeField] public string Name {  get; private set; }
    [field: SerializeField] public GameObject Model {  get; private set; }
    [field: SerializeField] public Sprite Icon { get; private set; }
    [SerializeField] private bool isStackable = true;

    [Tooltip("Sets the limit for an item if the item is Stackable")]
    [SerializeField] private int stackLimit = 6;
    public int StackLimit
    {
        get => isStackable ? stackLimit : 1;
        private set => stackLimit = value;
    }
}

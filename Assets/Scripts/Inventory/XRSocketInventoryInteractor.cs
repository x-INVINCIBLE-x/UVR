using System;
using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.AffordanceSystem.Receiver.Primitives;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class XRSocketInventoryInteractor : UnityEngine.XR.Interaction.Toolkit.Interactors.XRSocketInteractor
{
    [field: SerializeField] public string ID {  get; private set; }
    public TargetTag targetTag;
    public Item currentWeapon = null;
    public Transform meleeAttatchTransform;
    public Transform rangedAttachTransform;

    public UIToogleHandler closeHandler;
    private InventoryManager inventoryManager;
    private bool isSafe = false;
    private bool hasWeaponInSlot = false;

    private float spawnDelay = 0f;
    private Quaternion spawnRotation;

    protected override void Awake()
    {
        base.Awake();
        inventoryManager = InventoryManager.Instance;
        closeHandler = GetComponentInParent<UIToogleHandler>();
    }

#if UNITY_EDITOR
    [ContextMenu("Generate UID")]
    private void GenerateUID()
    {
        if (Application.IsPlaying(gameObject)) return;
        if (string.IsNullOrEmpty(gameObject.scene.path)) return;

        SerializedObject serializedObject = new SerializedObject(this);
        SerializedProperty property = serializedObject.FindProperty("id");

        if (string.IsNullOrEmpty(property.stringValue))
        {
            property.stringValue = Guid.NewGuid().ToString();
            serializedObject.ApplyModifiedProperties();
        }
    }
#endif
    public override bool CanHover(UnityEngine.XR.Interaction.Toolkit.Interactables.IXRHoverInteractable interactable)
    {
        return base.CanHover(interactable) && interactable.transform.CompareTag(targetTag.ToString());
    }

    public override bool CanSelect(UnityEngine.XR.Interaction.Toolkit.Interactables.IXRSelectInteractable interactable)
    {
        return base.CanSelect(interactable) && interactable.transform.CompareTag(targetTag.ToString());
    }

    protected override void OnSelectEntered(SelectEnterEventArgs args)
    {
        base.OnSelectEntered(args);

        if (args.interactableObject.transform.TryGetComponent(out Item weapon))
        {
            currentWeapon = weapon;
        }

        if (hasWeaponInSlot)
        {
            return;
        }

        hasWeaponInSlot = true;
        InventoryItem item = new InventoryItem(weapon.data);
        InventoryManager.Instance.AddItemFromSocket(item, ID);
        //inventoryManager.AddItemFromSocket(weapon, this);
    }

    protected override void OnSelectExited(SelectExitEventArgs args)
    {
        base.OnSelectExited(args);

        if (!isSafe)
        {
            return;
        }

        InventoryManager.Instance.RemoveItemFromSocket(ID);
        currentWeapon = null;
        hasWeaponInSlot = false;
        //inventoryManager.RemoveItemFromSocket(this);
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        closeHandler.OnClose += HandleClose;

        isSafe = true;
        StartCoroutine(SpawnWeapon());
    }

    private IEnumerator SpawnWeapon()
    {
        //yield return new WaitForSeconds(spawnDelay);

        yield return null;

        InventoryItem item = InventoryManager.Instance.GetItem(ID);
        if (item != null)
        {
            currentWeapon = Instantiate(item.data.Model, transform.position + new Vector3(0, 0.1f, 0), spawnRotation).GetComponent<Item>();
            if (currentWeapon.TryGetComponent(out PurchasableItem purchasable))
            {
                Destroy(purchasable);
            }
        }
    }

    private void HandleClose()
    {
        isSafe = false;
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        closeHandler.OnClose -= HandleClose;

        if (currentWeapon == null)
        {
            return;
        }

        spawnRotation = currentWeapon.transform.rotation;

        Destroy(currentWeapon.gameObject);
        currentWeapon = null;
    }
}
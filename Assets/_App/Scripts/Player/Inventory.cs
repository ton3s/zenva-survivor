using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.Events;

public class Inventory : MonoBehaviour
{
	public ItemSlotUI[] uiSlots;
	public ItemSlot[] slots;

	public GameObject inventoryWindow;
	public Transform dropPosition;

	[Header("Selected Item")]
	private ItemSlot selectedItem;
	private int selectedItemIndex;
	public TextMeshProUGUI selectedItemName;
	public TextMeshProUGUI selectedItemDescription;
	public TextMeshProUGUI selectedItemStatNames;
	public TextMeshProUGUI selectedItemStatValues;
	public GameObject useButton;
	public GameObject equipButton;
	public GameObject unEquipButton;
	public GameObject dropButton;

	private int curEquipIndex;

	// Components
	private PlayerController controller;

	[Header("Events")]
	public UnityEvent onOpenInventory;
	public UnityEvent onCloseInventory;

	// Singleton
	public static Inventory Instance;

	void Awake()
	{
		Instance = this;
		controller = GetComponent<PlayerController>();
	}

	void Start()
	{
		inventoryWindow.SetActive(false);
		slots = new ItemSlot[uiSlots.Length];

		// Initialize slots
		for (int i = 0; i < slots.Length; i++)
		{
			slots[i] = new ItemSlot();
			uiSlots[i].index = i;
			uiSlots[i].Clear();
		}
	}

	public void Toggle()
	{
		if (IsOpen())
		{
			inventoryWindow.SetActive(false);
		}
		else
		{
			inventoryWindow.SetActive(true);
		}
	}

	public bool IsOpen()
	{
		return inventoryWindow.activeInHierarchy;
	}

	// Add item to the inventory
	public void AddItem(ItemData item)
	{
		// Debug.Log($"AddItem : {item}");
		if (item.canStack)
		{
			ItemSlot slotToStackTo = GetItemStack(item);
			if (slotToStackTo != null)
			{
				slotToStackTo.quantity++;
				UpdateUI();
				return;
			}
		}

		ItemSlot emptySlot = GetEmptySlot();
		if (emptySlot != null)
		{
			emptySlot.item = item;
			emptySlot.quantity = 1;
			UpdateUI();
			return;
		}

		// Inventory is full and item cannot be stored
		ThrowItem(item);
	}

	// Spawn item in front of the player
	void ThrowItem(ItemData item)
	{
		Vector3 randomRotationEulerAngles = Vector3.one * Random.value * 360f;
		Instantiate(item.dropPrefab, dropPosition.position, Quaternion.Euler(randomRotationEulerAngles));
	}

	// Update UI when we add, throw or equip an item
	void UpdateUI()
	{
		for (int i = 0; i < slots.Length; i++)
		{
			if (slots[i].item != null)
			{
				uiSlots[i].Set(slots[i]);
			}
			else
			{
				uiSlots[i].Clear();
			}
		}
	}

	// Returns the item slot that the requested item can be stacked on
	// Returns null if there is no stack available
	ItemSlot GetItemStack(ItemData item)
	{
		for (int i = 0; i < slots.Length; i++)
		{
			if (slots[i].item == item && slots[i].quantity <= item.maxStackAmount)
			{
				return slots[i];
			}
		}
		return null;
	}

	// Returns an empty slot in the inventory
	// If there is no empty slot return null
	ItemSlot GetEmptySlot()
	{
		for (int i = 0; i < slots.Length; i++)
		{
			if (slots[i].item == null)
			{
				return slots[i];
			}
		}
		return null;
	}

	// Called when we click on an item slot
	public void SelectItem(int index)
	{

	}

	// Called when the inventory opens or the currently selected item has depleted
	void ClearSelectedItemWindow()
	{

	}

	public void OnUseButton()
	{

	}

	public void OnEquipButton()
	{

	}

	void UnEquip(int index)
	{

	}

	public void OnUnEquipButton()
	{

	}

	public void OnDropButton()
	{

	}

	void RemoveSelectedItem()
	{

	}

	public void RemoveItem(ItemData item)
	{

	}

	public bool HasItems(ItemData item, int quantity)
	{
		return false;
	}
}

public class ItemSlot
{
	public ItemData item;
	public int quantity;
}

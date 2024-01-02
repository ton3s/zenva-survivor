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
	private PlayerNeeds needs;

	[Header("Events")]
	public UnityEvent onOpenInventory;
	public UnityEvent onCloseInventory;

	// Singleton
	public static Inventory Instance;

	void Awake()
	{
		Instance = this;
		controller = GetComponent<PlayerController>();
		needs = GetComponent<PlayerNeeds>();
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

		ClearSelectedItemWindow();
	}

	public void onInventoryButton(InputAction.CallbackContext context)
	{
		if (context.phase == InputActionPhase.Started)
		{
			Toggle();
		}
	}

	public void Toggle()
	{
		if (IsOpen())
		{
			// Closing inventory
			inventoryWindow.SetActive(false);
			onCloseInventory.Invoke();
			controller.ToggleCursor(false);
		}
		else
		{
			// Open inventory
			inventoryWindow.SetActive(true);
			onOpenInventory.Invoke();
			ClearSelectedItemWindow();
			controller.ToggleCursor(true);
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
		// Check if there is an item in the slot
		if (slots[index].item == null)
		{
			return;
		}

		selectedItem = slots[index];
		selectedItemIndex = index;

		selectedItemName.text = selectedItem.item.displayName;
		selectedItemDescription.text = selectedItem.item.description;

		// Set stat values and stat names
		selectedItemStatNames.text = string.Empty;
		selectedItemStatValues.text = string.Empty;
		for (int i = 0; i < selectedItem.item.consumables.Length; i++)
		{
			selectedItemStatNames.text += selectedItem.item.consumables[i].type.ToString() + "\n";
			selectedItemStatValues.text += selectedItem.item.consumables[i].value.ToString() + "\n";
		}

		useButton.SetActive(selectedItem.item.type == ItemType.Consumable);
		equipButton.SetActive(selectedItem.item.type == ItemType.Equipable && !uiSlots[index].equipped);
		unEquipButton.SetActive(selectedItem.item.type == ItemType.Equipable && uiSlots[index].equipped);
		dropButton.SetActive(true);
	}

	// Called when the inventory opens or the currently selected item has depleted
	void ClearSelectedItemWindow()
	{
		// Clear the text
		selectedItem = null;
		selectedItemName.text = string.Empty;
		selectedItemDescription.text = string.Empty;
		selectedItemStatNames.text = string.Empty;
		selectedItemStatValues.text = string.Empty;

		// Disable buttons
		useButton.SetActive(false);
		equipButton.SetActive(false);
		unEquipButton.SetActive(false);
		dropButton.SetActive(false);
	}

	public void OnUseButton()
	{
		if (selectedItem.item.type == ItemType.Consumable)
		{
			for (int i = 0; i < selectedItem.item.consumables.Length; i++)
			{
				ItemDataConsumable consumable = selectedItem.item.consumables[i];
				switch (consumable.type)
				{
					case ConsumableType.Health:
						Debug.Log("Health");
						needs.Heal(consumable.value);
						break;
					case ConsumableType.Hunger:
						Debug.Log("Hunger");
						needs.Eat(consumable.value);
						break;
					case ConsumableType.Thirst:
						Debug.Log("Thirst");
						needs.Drink(consumable.value);
						break;
					case ConsumableType.Sleep:
						Debug.Log("Sleep");
						needs.Sleep(consumable.value);
						break;
				}
			}
		}

		RemoveSelectedItem();
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
		ThrowItem(selectedItem.item);
		RemoveSelectedItem();
	}

	void RemoveSelectedItem()
	{
		selectedItem.quantity--;

		// If this is the last remaining item, remove from inventory
		if (selectedItem.quantity == 0)
		{
			if (uiSlots[selectedItemIndex].equipped)
			{
				UnEquip(selectedItemIndex);
			}

			selectedItem.item = null;
			ClearSelectedItemWindow();
		}

		UpdateUI();
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

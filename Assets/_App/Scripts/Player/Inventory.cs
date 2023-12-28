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
	public static Inventory instance;

	void Awake()
	{
		instance = this;
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
}

public class ItemSlot
{
	public ItemData item;
	public int quantity;
}

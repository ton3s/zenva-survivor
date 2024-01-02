using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ItemSlotUI : MonoBehaviour
{
	public Button button;
	public Image icon;
	public TextMeshProUGUI quantityText;
	private ItemSlot curSlot;
	private Outline outline;

	public int index;
	public bool equipped;

	void Awake()
	{
		outline = GetComponent<Outline>();
	}

	void OnEnable()
	{
		outline.enabled = equipped;
	}

	public void Set(ItemSlot slot)
	{
		if (slot.item == null) return;
		curSlot = slot;
		icon.gameObject.SetActive(true);

		// Debug.Log($"Setting item : {slot.item}");
		icon.sprite = slot.item.icon;
		quantityText.text = slot.quantity > 1 ? slot.quantity.ToString() : string.Empty;
		if (outline != null)
		{
			outline.enabled = equipped;
		}
	}

	public void Clear()
	{
		curSlot = null;
		icon.gameObject.SetActive(false);
		quantityText.text = string.Empty;
	}

	public void OnButtonClick()
	{
		// Debug.Log("Item Slot UI Button Selected");
		Inventory.Instance.SelectItem(index);
	}
}

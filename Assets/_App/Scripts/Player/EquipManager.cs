using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class EquipManager : MonoBehaviour
{
	public Equip curEquip; // Keep track of current equipped object
	public Transform equipParent; // Equip camera

	// Components
	private PlayerController controller;

	// Singleton
	public static EquipManager Instance;


	void Awake()
	{
		Instance = this;
		controller = GetComponent<PlayerController>();
	}

	void Start()
	{
	}

	public void OnAttackInput(InputAction.CallbackContext context)
	{
		if (context.phase == InputActionPhase.Performed
			&& curEquip != null
			&& controller.canLook == true)
		{
			curEquip.OnAttackInput();
		}
	}

	public void OnAltAttackInput(InputAction.CallbackContext context)
	{
		if (context.phase == InputActionPhase.Performed
			&& curEquip != null
			&& controller.canLook == true)
		{
			curEquip.OnAltAttackInput();
		}
	}

	public void EquipNew(ItemData item)
	{
		UnEquip();
		curEquip = Instantiate(item.equipPrefab, equipParent).GetComponent<Equip>();
	}

	public void UnEquip()
	{
		if (curEquip != null)
		{
			Destroy(curEquip.gameObject);
			curEquip = null;
		}
	}
}

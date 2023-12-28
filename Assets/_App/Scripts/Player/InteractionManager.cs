using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;

public class InteractionManager : MonoBehaviour
{
	public float checkRate = 0.05f;
	private float lastCheckTime;
	public float maxCheckDistance;
	public LayerMask layerMask;

	private GameObject curInteractGameObject;
	private IInteractable curInteractable;

	public TextMeshProUGUI promptText;
	private Camera cam;

	void Start()
	{
		cam = Camera.main;
	}

	void Update()
	{
		if (Time.time - lastCheckTime > checkRate)
		{
			lastCheckTime = Time.time;

			// Get a ray from the center of the screen
			Ray ray = cam.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2));
			RaycastHit hit;

			if (Physics.Raycast(ray, out hit, maxCheckDistance, layerMask))
			{
				// Check if the object is not current selected
				if (hit.collider.gameObject != curInteractGameObject)
				{
					curInteractGameObject = hit.collider.gameObject;
					curInteractable = hit.collider.GetComponent<IInteractable>();
					SetPromptText();
				}
			}
			else
			{
				// If not object is selected
				curInteractGameObject = null;
				curInteractable = null;
				promptText.gameObject.SetActive(false);
			}
		}
	}

	void SetPromptText()
	{
		promptText.gameObject.SetActive(true);
		promptText.text = string.Format("<b>[E]</b> {0}", curInteractable.GetInteractPrompt());
	}

	public void OnInteractInput(InputAction.CallbackContext context)
	{
		if (context.phase == InputActionPhase.Started && curInteractable != null)
		{
			curInteractable.OnInteract();
			curInteractGameObject = null;
			curInteractable = null;
			promptText.gameObject.SetActive(false);
		}
	}
}

public interface IInteractable
{
	string GetInteractPrompt();
	void OnInteract();
}

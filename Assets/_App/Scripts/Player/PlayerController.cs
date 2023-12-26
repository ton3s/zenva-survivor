using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
	[Header("Movement")]
	public float moveSpeed;
	private Vector2 curMovementInput;

	[Header("Look")]
	public Transform cameraContainer;
	public float minXLook;
	public float maxXLook;
	private float camCurXRot;
	public float lookSensitivity;

	private Vector2 mouseDelta;

	// Components
	private Rigidbody rig;

	void Awake()
	{
		rig = GetComponent<Rigidbody>();
	}

	void Start()
	{
		Cursor.lockState = CursorLockMode.Locked;
	}

	void FixedUpdate()
	{
		Move();
	}

	void LateUpdate()
	{
		// Rotate the camera after the player has moved
		CameraLook();
	}

	void Move()
	{
		// x/z axis are controlled independently or vertical movement (gravity)
		Vector3 dir = transform.forward * curMovementInput.y + transform.right * curMovementInput.x;
		dir *= moveSpeed;

		// Allows us to fall with gravity
		dir.y = rig.velocity.y;
		rig.velocity = dir;
	}

	void CameraLook()
	{
		// Handle look up/down
		camCurXRot += mouseDelta.y * lookSensitivity;
		camCurXRot = Mathf.Clamp(camCurXRot, minXLook, maxXLook);
		cameraContainer.localEulerAngles = new Vector3(-camCurXRot, 0, 0);

		// Handle left/right rotation
		transform.eulerAngles += new Vector3(0, mouseDelta.x * lookSensitivity, 0);
	}

	// Called when we move the mouse - managed by Input System
	public void OnLookInput(InputAction.CallbackContext context)
	{
		mouseDelta = context.ReadValue<Vector2>();
	}

	public void OnMoveInput(InputAction.CallbackContext context)
	{
		// Check if the action has been performed/cancelled
		if (context.phase == InputActionPhase.Performed)
		{
			curMovementInput = context.ReadValue<Vector2>();
		}
		else if (context.phase == InputActionPhase.Canceled)
		{
			curMovementInput = Vector2.zero;
		}
	}

}

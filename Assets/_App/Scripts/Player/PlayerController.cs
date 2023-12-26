using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
	[Header("Movement")]
	public float moveSpeed;
	private Vector2 curMovementInput;

	[Header("Jump")]
	public float jumpForce;
	public LayerMask groundLayerMask;

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

	public void OnJumpInput(InputAction.CallbackContext context)
	{
		// Is this the first frame we are pressing the button
		if (context.phase == InputActionPhase.Started)
		{
			if (IsGrounded())
			{
				// Add instantaneous force upwards
				rig.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
			}
		}
	}

	bool IsGrounded()
	{
		Ray[] rays = new Ray[4] {
			new Ray(transform.position + (transform.forward * 0.2f) + (Vector3.up * 0.01f), Vector3.down),
			new Ray(transform.position + (-transform.forward * 0.2f) + (Vector3.up * 0.01f), Vector3.down),
			new Ray(transform.position + (transform.right * 0.2f) + (Vector3.up * 0.01f), Vector3.down),
			new Ray(transform.position + (-transform.right * 0.2f) + (Vector3.up * 0.01f), Vector3.down)
		};

		for (int i = 0; i < rays.Length; i++)
		{
			if (Physics.Raycast(rays[i], 0.1f, groundLayerMask))
			{
				return true;
			}
		}
		return false;
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.red;
		Gizmos.DrawRay(transform.position + (transform.forward * 0.2f), Vector3.down);
		Gizmos.DrawRay(transform.position + (-transform.forward * 0.2f), Vector3.down);
		Gizmos.DrawRay(transform.position + (transform.right * 0.2f), Vector3.down);
		Gizmos.DrawRay(transform.position + (-transform.right * 0.2f), Vector3.down);
	}

}

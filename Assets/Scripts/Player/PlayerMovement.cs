using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using _conveyor.Mobile3DFPS.UI;
using Mirror;

namespace _conveyor.Mobile3DFPS.Player
{
	public class PlayerMovement : MonoBehaviour
	{
		public float walkSpeed = 8.0f;
		public float sprintSpeed = 16.0f;
		public float jumpSpeed = 10.0f;
		
		[Space]
		public float smoothTime;
		public float jumpForce;
		public float rotationSpeed;
		
		[Header("Ground Check Vars:"), Space]
		public Vector3 offset;
		public LayerMask groundMask;
		
		//privates
		private Vector3 refVel;
		private Vector3 vel;
		private Vector3 groundContactNormal;
		
		private Rigidbody rb;
		private bool isGrounded;
		private float turnV;
		private Camera cam;
		private float currentSpeed;
		private NetworkIdentity netIdentity;
		private UIManager ui;
		public bool isSprinting { get; set; }
	
		private void Start()
		{
			currentSpeed = walkSpeed;
			cam = Camera.main;
			rb = GetComponent<Rigidbody>();
			ui = FindObjectOfType<UIManager>();
			netIdentity = GetComponent<NetworkIdentity>();
			
			if(netIdentity.isLocalPlayer)
			{
				if(ui!=null)
				{
					ui.playerMovement = this;
				}
			}
		}
		
		private void FixedUpdate()
		{
			CheckGrounding();
			
			Vector3 input = new Vector3(ui.movementJoystick.Horizontal, 0.0f, ui.movementJoystick.Vertical);
			
			if (input.magnitude > 1f) input.Normalize();

			//facing camera
			input = Vector3.ProjectOnPlane(input, groundContactNormal);
			float targetangle = Mathf.Atan2(input.x, input.z) * Mathf.Rad2Deg + cam.transform.eulerAngles.y;
			float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetangle, ref turnV, rotationSpeed);
			transform.rotation = Quaternion.Euler(0.0f, angle, 0.0f);
			
			if(input.magnitude != 0.0f)
			{
				//apply vars to the rigidbody...
				Vector3 moveDir = new Vector3();
				currentSpeed = isGrounded ? walkSpeed : jumpSpeed;
				moveDir = Quaternion.Euler(0.0f, targetangle, 0.0f) * Vector3.forward * (isSprinting ? sprintSpeed : currentSpeed);
				moveDir.y = rb.velocity.y;
				rb.velocity = moveDir;
			}
		}
		
		public void Jump()
		{
			if(isGrounded)
				rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
		}	
		
		private void CheckGrounding()
		{
			RaycastHit hit;
			isGrounded = Physics.Raycast(transform.position + offset, Vector3.down, out hit, 0.5f, groundMask, QueryTriggerInteraction.Ignore);
		
			if(hit.collider!=null) {
				groundContactNormal = hit.normal;
			}
		}
		
		// Implement this OnDrawGizmosSelected if you want to draw gizmos only if the object is selected.
		private void OnDrawGizmosSelected() {
			Gizmos.color = Color.yellow;
			Gizmos.DrawWireSphere(transform.position + offset, 0.2f);
		}
	}
}







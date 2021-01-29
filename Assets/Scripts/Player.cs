using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
	private Rigidbody myRigidBody;

	public bool Grounded;
	public float MoveSpeed;
	public float JumpPower;

	// Start is called before the first frame update
	void Start()
	{
		myRigidBody = GetComponent<Rigidbody>();
	}

	// Update is called once per frame
	void Update()
	{
		UpdateInput();
		UpdateOrientation();
		UpdateGrounded();
	}

	/// <summary>
	/// Reads input from the player for movement.
	/// </summary>
	private void UpdateInput()
	{
		Vector3 velocity = myRigidBody.velocity;

		if (Input.GetKey(KeyCode.W))
		{
			velocity.z = MoveSpeed;
		}
		else if (Input.GetKey(KeyCode.S))
		{
			velocity.z = -MoveSpeed;
		}
		else
		{
			velocity.z = 0f;
		}

		if (Input.GetKey(KeyCode.A))
		{
			velocity.x = -MoveSpeed;
		}
		else if (Input.GetKey(KeyCode.D))
		{
			velocity.x = MoveSpeed;
		}
		else
		{
			velocity.x = 0f;
		}

		if (Input.GetKeyDown(KeyCode.Space) && Grounded)
		{
			velocity.y = JumpPower;
		}

		myRigidBody.velocity = velocity;
	}

	/// <summary>
	/// Will keep the player facing the direction they move
	/// </summary>
	private void UpdateOrientation()
	{
		if (myRigidBody.velocity.sqrMagnitude > 1f)
		{
			transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(myRigidBody.velocity.normalized, Vector3.up), 10f * Time.deltaTime);
		}
	}

	/// <summary>
	/// Simple raycast check to see if the player is touching the ground.
	/// </summary>
	private void UpdateGrounded()
	{
		Grounded = Physics.Raycast(transform.position, Vector3.down, 0.75f);
	}
}

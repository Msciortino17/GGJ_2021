using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

public class Torpedo : MonoBehaviour
{
	public float MoveSpeed;
	public float TrackingSpeed;
	public float MaxTime;
	private float timer;

	public GameObject BoomPrefab;
	private Transform target;

	// Start is called before the first frame update
	void Start()
	{
		timer = MaxTime;
	}

	// Update is called once per frame
	void Update()
	{
		timer -= Time.deltaTime;
		if (timer < 0f)
		{
			DestroyTorpedo();
		}

		UpdateMovement();
	}

	/// <summary>
	/// Destroys the torpedo when it hits something.
	/// </summary>
	private void OnCollisionEnter(Collision collision)
	{
		Transform boom = Instantiate(BoomPrefab).transform;
		boom.position = transform.position;
		DestroyTorpedo();
	}

	/// <summary>
	/// Handles movement of the torpedo.
	/// </summary>
	private void UpdateMovement()
	{
		// Simply move forward
		transform.Translate(0f, 0f, MoveSpeed * Time.deltaTime, Space.Self);

		// Look towards a target if applicable.
		if (target != null)
		{
			Vector3 toTarget = (target.position - transform.position).normalized;
			transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(toTarget, Vector3.up), TrackingSpeed * Time.deltaTime);
		}
	}

	/// <summary>
	/// Destroys the gameobject and any effects.
	/// </summary>
	private void DestroyTorpedo()
	{
		Destroy(gameObject);
	}

	/// <summary>
	/// Locks onto the given target.
	/// </summary>
	public void SetTarget(Transform _target)
	{
		target = _target;
	}
}

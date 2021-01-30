using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

public class Torpedo : MonoBehaviour
{
	public float MoveSpeed;
	public float TrackingSpeed;
	public float MaxTime;
	public float HeatSeekingAccuracyRequired = 0.7f;
	public float HeatSeekingRange = 30.0f;
	private float timer;

	private bool hasTarget;
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

		UpdateTarget();
		UpdateMovement();
	}

	/// <summary>
	/// Destroys the torpedo when it hits something.
	/// </summary>
	private void OnCollisionEnter(Collision collision)
	{
		DestroyTorpedo();
	}

	/// <summary>
	/// Finds a target for the torpedo
	/// </summary>
	private void UpdateTarget()
	{
		if(hasTarget)
		{
			if(target == null) SetTarget(null); // Double check, because without this, it was throwing errors when one torpedo kills a shark

			return;
		}

		var heatSeekables = FindObjectsOfType<MonoBehaviour>()
								.OfType<IHeatSeekable>()
								.Select(x => ((MonoBehaviour)x).gameObject)
								.Where(x => 
								{
									Vector3 toTarget = (x.transform.position - transform.position);

									return toTarget.magnitude < HeatSeekingRange
											&& Vector3.Dot(toTarget.normalized, transform.forward) > HeatSeekingAccuracyRequired;
								} );
		
		GameObject newTarget = heatSeekables.FirstOrDefault();
		if(newTarget != null) SetTarget( newTarget.transform );
	}

	/// <summary>
	/// Handles movement of the torpedo.
	/// </summary>
	private void UpdateMovement()
	{
		// Simply move forward
		transform.Translate(0f, 0f, MoveSpeed * Time.deltaTime, Space.Self);

		// Look towards a target if applicable.
		if (hasTarget)
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
		hasTarget = _target != null;
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TorpedoTracking : MonoBehaviour
{
	[SerializeField] private Torpedo myTorpedo;

	// Start is called before the first frame update
	void Start()
	{

	}

	// Update is called once per frame
	void Update()
	{

	}

	/// <summary>
	/// If this touches something that can be targetted, have the main torpedo begin tracking it.
	/// </summary>
	/// <param name="other"></param>
	private void OnTriggerEnter(Collider other)
	{
		TorpedoTarget target = other.GetComponent<TorpedoTarget>();
		if (target != null)
		{
			myTorpedo.SetTarget(other.transform);
		}
	}
}

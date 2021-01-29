using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectableArtifact : MonoBehaviour
{
	public int ArtifactIndex;

	// Start is called before the first frame update
	void Start()
	{

	}

	// Update is called once per frame
	void Update()
	{

	}

	/// <summary>
	/// Simply collects the artifact in the inventory, then destroys this object.
	/// </summary>
	private void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("Submarine"))
		{
			Inventory inventory = other.GetComponent<Inventory>();
			inventory.CollectArtifact(ArtifactIndex);
			Destroy(gameObject);
		}
	}
}

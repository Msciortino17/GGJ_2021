using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinalGate : MonoBehaviour
{
	[SerializeField] private Inventory inventory;
	[SerializeField] private GameObject door;

	// Start is called before the first frame update
	void Start()
	{

	}

	// Update is called once per frame
	void Update()
	{
		if (inventory.HasArtifact(3))
		{
			door.SetActive(false);
		}
	}
}

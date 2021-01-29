using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
	public delegate void SimpleCallback();
	public SimpleCallback CollectedArtifactCallback;

	public int NumArtifacts;
	public bool[] CollectedArtifacts;

	// Start is called before the first frame update
	void Start()
	{
		CollectedArtifacts = new bool[NumArtifacts];
	}

	// Update is called once per frame
	void Update()
	{
		if (Input.GetKeyDown(KeyCode.M))
		{
			CollectArtifact(0);
		}
		if (Input.GetKeyDown(KeyCode.N))
		{
			CollectArtifact(1);
		}
		if (Input.GetKeyDown(KeyCode.B))
		{
			CollectArtifact(2);
		}
		if (Input.GetKeyDown(KeyCode.V))
		{
			CollectArtifact(3);
		}
	}

	public bool HasArtifact(int _index)
	{
		if (_index >= 0 && _index < NumArtifacts)
		{
			return CollectedArtifacts[_index];
		}
		return false;
	}

	public void CollectArtifact(int _index)
	{
		if (_index >= 0 && _index < NumArtifacts)
		{
			CollectedArtifacts[_index] = true;
			CollectedArtifactCallback?.Invoke();
		}
	}
}

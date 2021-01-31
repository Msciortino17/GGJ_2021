using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyAfterTime : MonoBehaviour
{
	public bool Countdown;
	public float Timer;

	// Start is called before the first frame update
	void Start()
	{

	}

	// Update is called once per frame
	void Update()
	{
		if (Countdown)
		{
			Timer -= Time.deltaTime;
			if (Timer <= 0f)
			{
				Destroy(gameObject);
			}
		}
	}
}

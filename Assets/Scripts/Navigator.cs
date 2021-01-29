using UnityEngine;
using UnityEngine.AI;

public class Navigator : MonoBehaviour
{
	public Camera Cam;

	public NavMeshAgent Agent;

	// Update is called once per frame
	void Update()
	{
		if(Input.GetMouseButtonDown(0))
		{
			Ray ray = Cam.ScreenPointToRay(Input.mousePosition);
			RaycastHit hit;

			if( Physics.Raycast(ray, out hit) )
			{
				Agent.SetDestination( hit.point );
			}
		}
	}
}

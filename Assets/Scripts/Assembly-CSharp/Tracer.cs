using UnityEngine;

public class Tracer
{
	public GameObject tracerObject;

	public Transform tracerStart;

	public Transform tracerEnd;

	public Transform tracerHead;

	public Tracer(GameObject tracerObj)
	{
		if (tracerObj != null)
		{
			tracerObject = tracerObj;
			tracerStart = tracerObj.transform.Find("start");
			tracerEnd = tracerObj.transform.Find("end");
			tracerHead = tracerEnd.Find("Tracer_Head");
		}
	}
}

using System.Collections.Generic;
using UnityEngine;

public class KOTHControllerTracker : MonoBehaviour
{
	public List<PlayerController> controllersInCollider = new List<PlayerController>();

	private void OnEnable()
	{
		controllersInCollider.Clear();
	}

	private void OnCollisionEnter(Collision c)
	{
		PlayerController component = c.gameObject.GetComponent<PlayerController>();
		if (component != null && !controllersInCollider.Contains(component))
		{
			controllersInCollider.Add(component);
		}
	}

	private void OnCollisionExit(Collision c)
	{
		PlayerController component = c.gameObject.GetComponent<PlayerController>();
		if (component != null && controllersInCollider.Contains(component))
		{
			controllersInCollider.Remove(component);
		}
	}

	private void OnTriggerEnter(Collider c)
	{
		PlayerController component = c.gameObject.GetComponent<PlayerController>();
		if (component != null && !controllersInCollider.Contains(component))
		{
			controllersInCollider.Add(component);
		}
	}

	private void OnTriggerExit(Collider c)
	{
		PlayerController component = c.gameObject.GetComponent<PlayerController>();
		if (component != null && controllersInCollider.Contains(component))
		{
			controllersInCollider.Remove(component);
		}
	}
}

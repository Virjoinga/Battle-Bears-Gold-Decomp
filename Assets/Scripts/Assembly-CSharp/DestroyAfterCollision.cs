using UnityEngine;

public class DestroyAfterCollision : MonoBehaviour
{
	private void OnCollisionEnter(Collision collision)
	{
		Object.Destroy(base.gameObject);
	}

	private void OnTriggerEnter(Collider collider)
	{
		Object.Destroy(base.gameObject);
	}
}

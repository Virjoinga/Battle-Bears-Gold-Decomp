using UnityEngine;

public class SpawnGameObjectOnCollision : MonoBehaviour
{
	[SerializeField]
	private GameObject _objectToSpawn;

	private void OnCollisionEnter(Collision collision)
	{
		SpawnObject(collision.contacts[0].point);
	}

	private void OnTriggerEnter(Collider collider)
	{
		SpawnObject(collider.transform.position);
	}

	private void SpawnObject(Vector3 spawnPos)
	{
		if (_objectToSpawn != null)
		{
			Object.Instantiate(_objectToSpawn, spawnPos, base.transform.rotation);
		}
	}
}

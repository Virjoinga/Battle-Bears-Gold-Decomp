using UnityEngine;

public class SpawnOnDestroy : MonoBehaviour
{
	public GameObject objectToSpawn;

	private void OnDestroy()
	{
		if (objectToSpawn != null)
		{
			Object.Instantiate(objectToSpawn, base.transform.position, base.transform.rotation);
		}
	}
}

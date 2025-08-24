using UnityEngine;

public class GoopSpawner : ConfigurableNetworkObject
{
	public ConfigurableNetworkObject AOEDamage;

	public ConfigurableNetworkObject objectToSpawn;

	public Vector3[] objectRotation;

	public float velocity = 600f;

	public string netsyncNameOverride;

	private new void Start()
	{
		SpawnObjects();
	}

	private void SpawnObjects()
	{
		GameObject gameObject = Object.Instantiate(AOEDamage.gameObject, base.transform.position, base.transform.rotation) as GameObject;
		ConfigurableNetworkObject component = gameObject.GetComponent<ConfigurableNetworkObject>();
		component.OwnerID = base.OwnerID;
		component.DamageMultiplier = base.DamageMultiplier;
		component.SetItemOverride(netsyncNameOverride);
		component.SetEquipmentNames(equipmentNames);
		for (int i = 0; i < objectRotation.Length; i++)
		{
			GameObject gameObject2 = Object.Instantiate(objectToSpawn.gameObject, base.transform.position, base.transform.rotation) as GameObject;
			gameObject2.transform.forward = gameObject2.transform.up;
			gameObject2.transform.eulerAngles += objectRotation[i];
			ConfigurableNetworkObject component2 = gameObject2.GetComponent<ConfigurableNetworkObject>();
			component2.OwnerID = base.OwnerID;
			component2.DamageMultiplier = base.DamageMultiplier;
			component2.SetItemOverride(netsyncNameOverride);
			component2.SetEquipmentNames(equipmentNames);
			Rigidbody rigidbody = gameObject2.rigidbody;
			if (rigidbody != null)
			{
				rigidbody.velocity = gameObject2.transform.forward * velocity;
			}
		}
		Object.Destroy(base.gameObject);
	}
}

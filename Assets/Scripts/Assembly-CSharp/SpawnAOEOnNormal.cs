using UnityEngine;

[RequireComponent(typeof(Collider), typeof(Rigidbody))]
public class SpawnAOEOnNormal : SpawnAOE
{
	private Collision _collision;

	private void OnCollisionEnter(Collision col)
	{
		_collision = col;
		SpawnDamageAndAOE();
	}

	protected override void SpawnAOEObject()
	{
		ContactPoint contactPoint = FindClosestContactPoint(_collision);
		Quaternion rotation = Quaternion.FromToRotation(Vector3.up, contactPoint.normal);
		GameObject gameObject = Object.Instantiate(aoeDamage.gameObject, contactPoint.point, rotation) as GameObject;
		ConfigurableNetworkObject[] components = gameObject.GetComponents<ConfigurableNetworkObject>();
		ConfigurableNetworkObject[] array = components;
		foreach (ConfigurableNetworkObject configurableNetworkObject in array)
		{
			configurableNetworkObject.OwnerID = base.OwnerID;
			configurableNetworkObject.DamageMultiplier = base.DamageMultiplier;
			configurableNetworkObject.SetItemOverride(netsyncNameOverride);
			configurableNetworkObject.SetEquipmentNames(equipmentNames);
		}
	}

	private ContactPoint FindClosestContactPoint(Collision collision)
	{
		float num = float.MaxValue;
		ContactPoint result = default(ContactPoint);
		for (int i = 0; i < collision.contacts.Length; i++)
		{
			if ((int)hitMask == ((int)hitMask | (1 << collision.contacts[i].otherCollider.gameObject.layer)))
			{
				float num2 = Vector3.Distance(base.transform.position, collision.contacts[i].point);
				if (num2 < num)
				{
					num = num2;
					result = collision.contacts[i];
				}
			}
		}
		return result;
	}
}

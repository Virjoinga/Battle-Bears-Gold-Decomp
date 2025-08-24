using UnityEngine;

public class SpawnAOE : ConfigurableNetworkObject
{
	public ConfigurableNetworkObject instantDamage;

	public ConfigurableNetworkObject aoeDamage;

	public GameObject effect;

	public string netsyncNameOverride;

	public float raycastOffset = 15f;

	public LayerMask hitMask;

	[SerializeField]
	private bool _spawnOnStart = true;

	protected AudioSource _audio;

	protected override void Start()
	{
		base.Start();
		if (_spawnOnStart)
		{
			SpawnDamageAndAOE();
		}
		_audio = GetComponent<AudioSource>();
	}

	protected void SpawnDamageAndAOE()
	{
		SpawnInstantDamageObject();
		if (effect != null)
		{
			Object.Instantiate(effect, base.transform.position + Vector3.up * raycastOffset, effect.transform.rotation);
		}
		SpawnAOEObject();
		if (_audio != null && _audio.clip != null)
		{
			Object.Destroy(base.gameObject, _audio.clip.length);
		}
		else
		{
			Object.Destroy(base.gameObject);
		}
	}

	protected virtual void SpawnInstantDamageObject()
	{
		if (instantDamage != null)
		{
			GameObject gameObject = Object.Instantiate(instantDamage.gameObject, base.transform.position, base.transform.rotation) as GameObject;
			ConfigurableNetworkObject component = gameObject.GetComponent<ConfigurableNetworkObject>();
			component.OwnerID = base.OwnerID;
			component.DamageMultiplier = base.DamageMultiplier;
			component.SetItemOverride(netsyncNameOverride);
			component.SetEquipmentNames(equipmentNames);
		}
	}

	protected virtual void SpawnAOEObject()
	{
		RaycastHit hitInfo;
		if (Physics.Raycast(base.transform.position + Vector3.up * raycastOffset, Vector3.down, out hitInfo, float.PositiveInfinity, hitMask))
		{
			GameObject gameObject = Object.Instantiate(aoeDamage.gameObject, hitInfo.point, base.transform.rotation) as GameObject;
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
	}
}

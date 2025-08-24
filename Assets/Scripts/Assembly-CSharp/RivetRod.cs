using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RivetRod : DeployableObject
{
	private static List<RivetRod> deployedRods;

	private bool _isConnected;

	private bool _isMaster;

	private RivetRod _otherRod;

	private Transform _damageTransform;

	private Vector3 _myLastPosition;

	private Vector3 _otherLastPosition;

	public Transform raycastPoint;

	public float connectionDistance;

	public GameObject damageEffect;

	public LayerMask avoidMask;

	public ConfigurableNetworkObject impactDamage;

	public float timeToLive;

	public bool IsConnected
	{
		get
		{
			return _isConnected;
		}
		set
		{
			if (value)
			{
				StopCoroutine("DelayedDestroy");
				StartCoroutine("DelayedDestroy", effectDuration);
			}
			_isConnected = value;
		}
	}

	private void Awake()
	{
		configureItemName = spawnItemOverride;
		if (deployedRods == null)
		{
			deployedRods = new List<RivetRod>();
		}
		deployedRods.Add(this);
	}

	private new void Start()
	{
		ConfigureObject();
		StartCoroutine("DelayedDestroy", timeToLive);
	}

	private List<RivetRod> RodsInRange()
	{
		List<RivetRod> list = new List<RivetRod>();
		foreach (RivetRod deployedRod in deployedRods)
		{
			if (deployedRod != this && Vector3.Distance(deployedRod.transform.position, base.transform.position) < connectionDistance)
			{
				list.Add(deployedRod);
			}
		}
		list.Sort((RivetRod x, RivetRod y) => Vector3.Distance(base.transform.position, x.transform.position).CompareTo(Vector3.Distance(base.transform.position, y.transform.position)));
		return list;
	}

	private bool TryConnection(List<RivetRod> closeRods, out int rodId)
	{
		rodId = 0;
		foreach (RivetRod closeRod in closeRods)
		{
			if (!Physics.Linecast(raycastPoint.position, closeRod.raycastPoint.position, avoidMask) && !closeRod.IsConnected && closeRod.OwnerID == base.OwnerID)
			{
				return true;
			}
			rodId++;
		}
		rodId = -1;
		return false;
	}

	private void CreateDamageEffect(Vector3 connectedRaycastPoint)
	{
		GameObject gameObject = Object.Instantiate(damageEffect, base.transform.position, base.transform.rotation) as GameObject;
		_damageTransform = gameObject.transform;
		ResizeDamageObject(_damageTransform, connectedRaycastPoint);
		ConfigurableNetworkObject component = gameObject.GetComponent<ConfigurableNetworkObject>();
		component.OwnerID = base.OwnerID;
		component.DamageMultiplier = base.DamageMultiplier;
		ForwardSettings(component);
	}

	private void OnDestroy()
	{
		deployedRods.Remove(this);
	}

	private IEnumerator DelayedDestroy(float timeToLive)
	{
		yield return new WaitForSeconds(timeToLive);
		if (_isMaster && _damageTransform != null)
		{
			Object.Destroy(_damageTransform.gameObject);
		}
		Object.Destroy(base.gameObject);
	}

	private void ResizeDamageObject(Transform damageTransform, Vector3 connectedRaycastPoint)
	{
		Vector3 position = new Vector3((raycastPoint.position.x + connectedRaycastPoint.x) / 2f, (raycastPoint.position.y + connectedRaycastPoint.y) / 2f, (raycastPoint.position.z + connectedRaycastPoint.z) / 2f);
		float z = Vector3.Distance(raycastPoint.position, connectedRaycastPoint);
		damageTransform.position = position;
		damageTransform.LookAt(raycastPoint.transform);
		damageTransform.localScale = new Vector3(damageTransform.localScale.x, damageTransform.localScale.y, z);
	}

	public override void OnDestroyDeployable()
	{
		Object.Destroy(base.gameObject);
	}

	public override void OnDetonateDeployable(PlayerController triggeringPlayer, bool fromExplosion)
	{
		if (triggeringPlayer != null)
		{
			ConfigurableNetworkObject component = (Object.Instantiate(impactDamage.gameObject, base.transform.position, base.transform.rotation) as GameObject).GetComponent<ConfigurableNetworkObject>();
			component.OwnerID = base.OwnerID;
			component.DamageMultiplier = base.DamageMultiplier;
			ForwardSettings(component);
			Object.Destroy(base.gameObject);
		}
	}

	public override void ConfigureObject()
	{
		if (spawnItemOverride != string.Empty && ServiceManager.Instance != null)
		{
			Item itemByName = ServiceManager.Instance.GetItemByName(spawnItemOverride);
			itemByName.UpdateProperty("duration", ref effectDuration, base.EquipmentNames);
			itemByName.UpdateProperty("timeToLive", ref timeToLive, base.EquipmentNames);
			itemByName.UpdateProperty("range", ref connectionDistance, base.EquipmentNames);
			RotatingItem componentInChildren = base.gameObject.GetComponentInChildren<RotatingItem>();
			if (componentInChildren != null)
			{
				float num = (float)itemByName.properties["minDamageRange"] * 2f;
				componentInChildren.transform.localScale = new Vector3(num, num, num);
			}
		}
	}
}

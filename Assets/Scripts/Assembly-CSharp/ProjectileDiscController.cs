using System;
using ExitGames.Client.Photon;
using UnityEngine;

public class ProjectileDiscController : DiscController
{
	private static readonly string _discFireDelayStr = "maximum_disc_fires";

	[SerializeField]
	private GameObject _projectileToSpawn;

	[SerializeField]
	private Vector3 _velocity;

	[SerializeField]
	private float _firingDelay = 1f;

	[SerializeField]
	private float _maxDiscFires = 2f;

	private float _nextFiringTime;

	private float _discsFired;

	public override void Start()
	{
		base.Start();
		_nextFiringTime = Time.fixedTime + _firingDelay;
		if (ServiceManager.Instance != null)
		{
			Item itemByName = ServiceManager.Instance.GetItemByName(_itemOverride);
			if (itemByName != null && itemByName.properties.ContainsKey(_discFireDelayStr))
			{
				_maxDiscFires = (float)itemByName.properties[_discFireDelayStr];
			}
		}
		WeaponManagerBase weaponManager = base.Owner.WeaponManager;
		weaponManager.OnFirePrimary = (Action)Delegate.Combine(weaponManager.OnFirePrimary, new Action(OnFirePrimary));
	}

	public void OnFirePrimary()
	{
		if (base.Owner.WeaponManager.CurrentWeaponIndex == 0 && !base.IsRemote && base.Owner != null && base.Owner.Director != null && Time.fixedTime >= _nextFiringTime)
		{
			if (base.Owner.NetSync != null)
			{
				Hashtable hashtable = new Hashtable();
				hashtable[(byte)0] = base.Owner.OwnerID;
				hashtable[(byte)1] = base.DiscIndex;
				base.Owner.NetSync.SetAction(57, hashtable);
			}
			FireProjectile();
			_nextFiringTime = Time.fixedTime + _firingDelay;
			_discsFired += 1f;
			if (_discsFired >= _maxDiscFires)
			{
				Expired();
			}
		}
	}

	public void FireProjectile()
	{
		GameObject gameObject = UnityEngine.Object.Instantiate(_projectileToSpawn, base.transform.position, Quaternion.identity) as GameObject;
		if (gameObject != null && base.Deployer != null)
		{
			gameObject.BroadcastMessage("SetEquipmentNames", base.Deployer.EquipmentNames, SendMessageOptions.DontRequireReceiver);
			gameObject.BroadcastMessage("SetItemOverride", base.Deployer.name, SendMessageOptions.DontRequireReceiver);
			gameObject.GetComponent<Rigidbody>().velocity = base.Owner.bodyRotator.rotation * _velocity;
			gameObject.transform.LookAt(gameObject.transform.position + gameObject.GetComponent<Rigidbody>().velocity);
			NetworkObject componentInChildren = gameObject.GetComponentInChildren<NetworkObject>();
			componentInChildren.OwnerID = base.Owner.OwnerID;
			componentInChildren.DamageMultiplier = base.Owner.DamageMultiplier;
			Collider componentInChildren2 = gameObject.GetComponent<Collider>();
			if (componentInChildren2 == null)
			{
				componentInChildren2 = gameObject.GetComponentInChildren<Collider>();
			}
			if (componentInChildren2 != null)
			{
				Physics.IgnoreCollision(componentInChildren2, base.Owner.GetComponent<Collider>());
			}
		}
	}

	protected override void Expired()
	{
		WeaponManagerBase weaponManager = base.Owner.WeaponManager;
		weaponManager.OnFirePrimary = (Action)Delegate.Remove(weaponManager.OnFirePrimary, new Action(OnFirePrimary));
		Explode();
		if (base.Deployer != null)
		{
			base.Deployer.OnDiscDestroyed();
		}
		base.Expired();
	}
}

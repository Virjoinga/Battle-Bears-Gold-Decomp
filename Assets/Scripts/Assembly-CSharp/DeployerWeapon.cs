using System.Collections;
using UnityEngine;

public class DeployerWeapon : WeaponBase
{
	[SerializeField]
	private GameObject _objectPrefab;

	[SerializeField]
	private bool _spawnAtRootObject;

	[SerializeField]
	private bool _spawnWithNoRotation;

	[SerializeField]
	private bool _parentToRootObject;

	[SerializeField]
	private bool _animationCreatesDeployable;

	[SerializeField]
	private Transform _spawnPoint;

	protected string restoreSwitchingCoroutineName = "RestoreWeaponSwitching";

	protected override void Start()
	{
		base.Start();
		base.playerController.canSwitchWeapons = true;
		base.playerController.WeaponManager.isDisabled = false;
		if (_spawnPoint == null)
		{
			_spawnPoint = base.transform;
		}
	}

	public override void OnRemoteAttack(Vector3 pos, Vector3 vel, int delay, float charge)
	{
		if (vel == Vector3.one)
		{
			DeployObject(0);
		}
		else
		{
			base.OnRemoteAttack(pos, vel, delay, charge);
		}
	}

	public override void OnRemoteAttack(Vector3 pos, Vector3 vel, int delay)
	{
		if (vel == Vector3.one)
		{
			DeployObject(0);
		}
		else
		{
			base.OnRemoteAttack(pos, vel, delay);
		}
	}

	public override bool OnAttack()
	{
		if (myAnimation != null && myAnimation["fire"] != null)
		{
			myAnimation["fire"].layer = 1;
		}
		if (base.playerController == null)
		{
			base.playerController = myTransform.root.GetComponent(typeof(PlayerController)) as PlayerController;
		}
		if (!isRemote && base.playerController != null)
		{
			base.playerController.canSwitchWeapons = false;
			base.playerController.WeaponManager.isDisabled = true;
			if (isFireInLoopOut)
			{
				StartCoroutine(restoreSwitchingCoroutineName, firingTime);
			}
		}
		if (!_animationCreatesDeployable)
		{
			DeployObject(0);
		}
		SendFireMessage(base.transform.position, Vector3.zero);
		return base.OnAttack();
	}

	public virtual GameObject DeployObject(int fromAnimation = 1)
	{
		GameObject gameObject = null;
		if (isRemote || !requireGrounded || base.playerController.Motor.IsGrounded())
		{
			if (myAnimation != null && myAnimation["fire"] != null)
			{
				myAnimation["fire"].layer = 0;
			}
			if (!isRemote)
			{
				SendFireMessage(base.transform.position, Vector3.one);
			}
			else if (fromAnimation == 1)
			{
				return null;
			}
			Transform transform = ((!_spawnAtRootObject && !(_spawnPoint == null)) ? _spawnPoint : base.transform.root);
			Quaternion rotation = ((!_spawnWithNoRotation) ? transform.rotation : Quaternion.identity);
			gameObject = Object.Instantiate(_objectPrefab, transform.position, rotation) as GameObject;
			if (_parentToRootObject)
			{
				gameObject.transform.parent = base.transform.root;
			}
			ConfigurableNetworkObject componentInChildren = gameObject.GetComponentInChildren<ConfigurableNetworkObject>();
			if (componentInChildren != null)
			{
				componentInChildren.SetEquipmentNames(base.EquipmentNames);
				componentInChildren.SetItemOverride(base.name);
				if (base.playerController != null)
				{
					componentInChildren.DamageMultiplier = base.playerController.DamageMultiplier;
				}
				componentInChildren.OwnerID = base.OwnerID;
			}
		}
		if (!isRemote && !isFireInLoopOut)
		{
			base.playerController.canSwitchWeapons = true;
			base.playerController.WeaponManager.isDisabled = false;
		}
		return gameObject;
	}

	private IEnumerator RestoreWeaponSwitching(float time)
	{
		yield return new WaitForSeconds(time);
		base.playerController.canSwitchWeapons = true;
		base.playerController.WeaponManager.isDisabled = false;
	}
}

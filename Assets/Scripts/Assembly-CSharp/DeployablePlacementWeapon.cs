using System.Collections;
using UnityEngine;

public class DeployablePlacementWeapon : WeaponBase, IDeployableWeapon
{
	public GameObject deployable;

	public float effectDuration = 1f;

	public bool matchesOwnerTeamColor;

	public Vector2 uvOffsetForBlue;

	protected bool _isAttacking;

	public bool IsAttacking
	{
		get
		{
			return _isAttacking;
		}
	}

	public override void ConfigureWeapon(Item item)
	{
		item.UpdateProperty("duration", ref effectDuration, base.EquipmentNames);
		base.ConfigureWeapon(item);
	}

	protected override void Start()
	{
		base.Start();
		base.playerController.WeaponManager.OnCheckForWeaponHiding();
		TryRefillMissingAmmo();
		if (!matchesOwnerTeamColor || base.playerController.Team != Team.BLUE)
		{
			return;
		}
		MeshFilter[] componentsInChildren = GetComponentsInChildren<MeshFilter>();
		MeshFilter[] array = componentsInChildren;
		foreach (MeshFilter meshFilter in array)
		{
			Vector2[] array2 = new Vector2[meshFilter.mesh.uv.Length];
			array2 = meshFilter.mesh.uv;
			for (int j = 0; j < array2.Length; j++)
			{
				array2[j].x += uvOffsetForBlue.x;
				array2[j].y += uvOffsetForBlue.y;
			}
			meshFilter.mesh.uv = array2;
		}
	}

	private void TryRefillMissingAmmo()
	{
		int currentWeaponIndex = base.playerController.WeaponManager.CurrentWeaponIndex;
		int currentClipSize = base.playerController.WeaponManager.GetCurrentClipSize(currentWeaponIndex);
		if (!(reloadTime > 0f) || currentClipSize > clipSize)
		{
			return;
		}
		int num = 0;
		DeployableObject[] array = Object.FindObjectsOfType(typeof(DeployableObject)) as DeployableObject[];
		DeployableObject[] array2 = array;
		foreach (DeployableObject deployableObject in array2)
		{
			if (deployableObject == null)
			{
				Debug.LogError("deployable is null, DeplyablePlacementWeapon:36");
			}
			else if (deployableObject.OwningPlayer != null && deployableObject.OwningPlayer.OwnerID == base.OwnerID && deployableObject.weaponIndex == currentWeaponIndex)
			{
				num++;
			}
		}
		if (currentClipSize + num < clipSize)
		{
			int num2 = clipSize - (currentClipSize + num);
			base.playerController.WeaponManager.OnDelayedIncreaseAmmo(reloadTime * (float)(num2 / clipSize), num2);
		}
	}

	public override bool OnAttack()
	{
		base.OnAttack();
		if (base.playerController == null)
		{
			base.playerController = myTransform.root.GetComponentInChildren(typeof(PlayerController)) as PlayerController;
		}
		if (base.playerController != null)
		{
			if (!base.playerController.Motor.IsGrounded())
			{
				return false;
			}
			if (deployable != null)
			{
				Vector3 position = base.playerController.BodyAnimator.transform.position;
				StartCoroutine(placingDeployableDelay(position, base.playerController.BodyAnimator.transform.eulerAngles, 0f));
				if (base.NetSyncReporter != null && !dontSendNetworkMessages)
				{
					base.NetSyncReporter.SpawnProjectile(position, base.playerController.BodyAnimator.transform.eulerAngles);
				}
			}
		}
		_isAttacking = true;
		return true;
	}

	private IEnumerator placingDeployableDelay(Vector3 pos, Vector3 rot, float delay)
	{
		if (!base.playerController.isRemote)
		{
			base.playerController.isDisabled = true;
			base.playerController.Motor.SetVelocity(Vector3.zero);
			base.playerController.Motor.SetControllable(false);
			base.playerController.WeaponManager.isDisabled = true;
		}
		yield return new WaitForSeconds(base.playerController.BodyAnimator.GetDeployableLayingInTime() - delay);
		base.playerController.WeaponManager.OnCheckForWeaponHiding();
		if (!base.playerController.IsDead)
		{
			createDeployable(pos, rot);
		}
		yield return new WaitForSeconds(base.playerController.BodyAnimator.GetDeployableLayingOutTime());
		if (!base.playerController.isRemote)
		{
			base.playerController.isDisabled = false;
			base.playerController.Motor.SetControllable(true);
			base.playerController.WeaponManager.isDisabled = false;
		}
		_isAttacking = false;
	}

	protected void OnDestroy()
	{
		if (base.playerController != null && !base.playerController.isRemote)
		{
			base.playerController.isDisabled = false;
			base.playerController.Motor.SetControllable(true);
			base.playerController.WeaponManager.isDisabled = false;
		}
	}

	public override void OnRemoteAttack(Vector3 pos, Vector3 rot, int delay)
	{
		base.OnRemoteAttack(pos, rot, delay);
		if (base.playerController == null)
		{
			base.playerController = myTransform.root.GetComponentInChildren(typeof(PlayerController)) as PlayerController;
		}
		if (base.playerController != null)
		{
			StartCoroutine(placingDeployableDelay(pos, rot, (float)delay / 1000f));
		}
	}

	private GameObject createDeployable(Vector3 pos, Vector3 rotation)
	{
		GameObject gameObject = Object.Instantiate(deployable) as GameObject;
		gameObject.transform.position = pos;
		gameObject.transform.eulerAngles = rotation;
		gameObject.BroadcastMessage("SetEquipmentNames", base.EquipmentNames, SendMessageOptions.DontRequireReceiver);
		gameObject.BroadcastMessage("SetItemOverride", base.name, SendMessageOptions.DontRequireReceiver);
		DeployableObject componentInChildren = gameObject.GetComponentInChildren<DeployableObject>();
		componentInChildren.OwnerID = ownerID;
		componentInChildren.DamageMultiplier = base.playerController.DamageMultiplier;
		componentInChildren.weaponIndex = base.playerController.WeaponManager.CurrentWeaponIndex;
		componentInChildren.deployableIndex = base.playerController.WeaponManager.lastDeployableIndex++;
		componentInChildren.effectDuration = effectDuration;
		componentInChildren.OwningPlayer = base.playerController;
		return gameObject;
	}

	public override void OnReload()
	{
		if (!_isAttacking)
		{
			base.OnReload();
		}
	}
}

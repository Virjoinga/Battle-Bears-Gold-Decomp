using System;
using System.Collections;
using UnityEngine;

public class DeployableTossWeapon : DeployableLauncherWeapon
{
	public override bool OnAttack()
	{
		if (base.playerController == null)
		{
			base.playerController = myTransform.root.GetComponentInChildren(typeof(PlayerController)) as PlayerController;
		}
		if (base.playerController != null)
		{
			if (base.playerController.WeaponManager.IsReloading)
			{
				return false;
			}
			if (projectile != null)
			{
				_isAttacking = true;
				StartCoroutine(tossingDeployable(base.OnAttack));
			}
		}
		return true;
	}

	public override void OnRemoteAttack(Vector3 pos, Vector3 rot, int delay)
	{
		if (base.playerController == null)
		{
			base.playerController = myTransform.root.GetComponentInChildren(typeof(PlayerController)) as PlayerController;
		}
		if (base.playerController != null && !base.playerController.WeaponManager.IsReloading)
		{
			StartCoroutine(remoteTossingDeployableDelay(pos, rot, (float)delay / 1000f, base.OnRemoteAttack));
		}
	}

	private IEnumerator remoteTossingDeployableDelay(Vector3 pos, Vector3 rot, float delay, Action<Vector3, Vector3, int> baseClassMethod)
	{
		yield return new WaitForSeconds(base.playerController.BodyAnimator.GetDeployableLayingInTime() - delay);
		base.playerController.WeaponManager.OnCheckForWeaponHiding();
		if (!base.playerController.IsDead && baseClassMethod != null)
		{
			baseClassMethod(pos, rot, 0);
		}
		yield return new WaitForSeconds(base.playerController.BodyAnimator.GetDeployableLayingOutTime());
	}

	private IEnumerator tossingDeployable(Func<bool> baseClassMethod)
	{
		yield return new WaitForSeconds(base.playerController.BodyAnimator.GetDeployableLayingInTime());
		base.playerController.WeaponManager.OnCheckForWeaponHiding();
		if (!base.playerController.IsDead && baseClassMethod != null)
		{
			baseClassMethod();
		}
		yield return new WaitForSeconds(base.playerController.BodyAnimator.GetDeployableLayingOutTime());
		_isAttacking = false;
		yield return null;
	}
}

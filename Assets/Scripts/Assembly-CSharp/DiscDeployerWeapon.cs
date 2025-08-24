using UnityEngine;

public class DiscDeployerWeapon : DeployerWeapon
{
	private int _currentIndex;

	private GameObject currDisc;

	public override GameObject DeployObject(int fromAnimation = 1)
	{
		currDisc = base.DeployObject(fromAnimation);
		GameObject gameObject = currDisc;
		if (gameObject == null)
		{
			return null;
		}
		DiscController component = gameObject.GetComponent<DiscController>();
		if (component != null)
		{
			component.FollowObject = base.transform.root;
			Physics.IgnoreCollision(component.collider, base.transform.root.collider);
			component.transform.rotation = Quaternion.identity;
			component.Owner = base.playerController;
			component.IsRemote = isRemote;
			component.DiscIndex = _currentIndex++;
			component.Deployer = this;
		}
		return gameObject;
	}

	public void DeployDisc()
	{
		DeployObject();
	}

	public override bool OnAttack()
	{
		if (currDisc != null)
		{
			currDisc.SendMessage("Expired");
		}
		return base.OnAttack();
	}

	public void OnDiscDestroyed()
	{
		if (!(base.playerController != null))
		{
			return;
		}
		base.playerController.canSwitchWeapons = true;
		if (base.playerController.WeaponManager != null)
		{
			base.playerController.WeaponManager.isDisabled = false;
		}
		StopCoroutine(restoreSwitchingCoroutineName);
		if (isAnimatingReload)
		{
			return;
		}
		StopCoroutine("DoFireInFireLoopFireOut");
		if (myAnimation != null)
		{
			myAnimation.Stop();
		}
		if (base.playerController.BodyAnimator != null)
		{
			base.playerController.BodyAnimator.StopCoroutine("DoFireInFireLoopFireOut");
			if (base.playerController.BodyAnimator.Animator != null)
			{
				base.playerController.BodyAnimator.Animator.Stop();
			}
			base.playerController.BodyAnimator.OnIdle();
		}
		if (base.playerController.WeaponManager != null && base.playerController.WeaponManager.CurrentWeapon == this)
		{
			base.playerController.WeaponManager.StopCoroutine("OnReload");
			base.playerController.WeaponManager.OnPlayReload(0f);
		}
	}
}

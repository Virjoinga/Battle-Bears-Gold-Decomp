public class LavaRifle : DeployableLauncherWeapon
{
	public override void OnReload()
	{
		base.OnReload();
		if (!_isAttacking)
		{
			(base.playerController.BodyAnimator as BodyAnimator).OnReload(reloadTime);
		}
	}
}

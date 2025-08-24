using UnityEngine;

public class VerticalMissleDeployerWeapon : DeployerWeapon
{
	private float lastDelayTime;

	public override void OnRemoteAttack(Vector3 pos, Vector3 vel, int delay)
	{
		lastDelayTime = (float)delay / 1000f;
		base.OnRemoteAttack(pos, vel, delay);
	}

	public override GameObject DeployObject(int fromAnimation)
	{
		GameObject gameObject = base.DeployObject(fromAnimation);
		if (isRemote && gameObject != null)
		{
			SimpleVerticalMissile component = gameObject.GetComponent<SimpleVerticalMissile>();
			component.AccountForNetworkDelay(lastDelayTime);
		}
		return gameObject;
	}

	public void DeployMissle(int fromAnimation)
	{
		DeployObject(fromAnimation);
	}
}

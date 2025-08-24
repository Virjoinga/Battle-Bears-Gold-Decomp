using UnityEngine;

public class BombDepositArea : MonoBehaviour
{
	public Team team;

	private Animation myAnimation;

	private void Awake()
	{
		myAnimation = base.animation;
	}

	protected void OnTriggerEnter(Collider c)
	{
		PlayerController componentInChildren = c.transform.root.GetComponentInChildren<PlayerController>();
		if (componentInChildren != null && !componentInChildren.isRemote && componentInChildren.Team != team && !componentInChildren.IsDead && componentInChildren.HasBomb)
		{
			componentInChildren.OnLocalDepositBomb();
			playDepositAnimation();
		}
	}

	public void OnRemoteBombDeposit(Team depositTeam)
	{
		if (depositTeam != team)
		{
			playDepositAnimation();
		}
	}

	private void playDepositAnimation()
	{
		base.audio.Play();
		myAnimation.Play("bombIn");
	}
}

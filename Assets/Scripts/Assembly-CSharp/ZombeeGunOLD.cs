using UnityEngine;

public class ZombeeGunOLD : RaycastWeapon
{
	private float targetAnimationPoint;

	private float currentAnimationPoint;

	protected override void Awake()
	{
		base.Awake();
		myAnimation["ammo"].layer = 2;
		myAnimation["ammo"].speed = 0f;
		myAnimation.Play("ammo");
	}

	public override void OnCurrentAmmo(int currentAmmo)
	{
		base.OnCurrentAmmo(currentAmmo);
		targetAnimationPoint = 1f - (float)currentAmmo / (float)clipSize;
		if (targetAnimationPoint == 0f)
		{
			currentAnimationPoint = 0f;
		}
	}

	private void Update()
	{
		currentAnimationPoint = Mathf.Lerp(currentAnimationPoint, targetAnimationPoint, Time.deltaTime * 5f);
		myAnimation["ammo"].normalizedTime = currentAnimationPoint;
	}
}

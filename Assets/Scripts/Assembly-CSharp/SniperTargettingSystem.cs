using System.Collections;
using UnityEngine;

public class SniperTargettingSystem : TargettingSystem
{
	public float targettingTimeLeft = 10f;

	private Animation myAnimation;

	protected override void Start()
	{
		base.Start();
		targettingSystemTransform.eulerAngles = new Vector3(0f, 180f, 0f);
		myAnimation = targettingSystem.GetComponent<Animation>();
	}

	protected override IEnumerator beginLock()
	{
		targettingSystem.SetActiveRecursively(true);
		if (myAnimation == null)
		{
			myAnimation = targettingSystem.GetComponent<Animation>();
		}
		myAnimation["sniperAni"].speed = myAnimation["sniperAni"].length / targettingTime;
		myAnimation.Play("sniperAni");
		isLocking = true;
		targettingTimeLeft = targettingTime;
		float timeInterval = 0.3f;
		while (targettingTimeLeft > timeInterval)
		{
			yield return new WaitForSeconds(timeInterval);
			targettingTimeLeft -= timeInterval;
		}
		targettingTimeLeft = 0f;
		lockedTarget = currentTarget;
		isLocking = false;
	}
}

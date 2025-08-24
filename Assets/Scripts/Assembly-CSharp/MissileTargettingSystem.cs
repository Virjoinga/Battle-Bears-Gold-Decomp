using System.Collections;
using UnityEngine;

public class MissileTargettingSystem : TargettingSystem
{
	private GameObject targettingSystemRed;

	private GameObject targettingSystemGreen;

	private Transform timeScaler;

	private float targettingTimeLeft;

	protected override void Start()
	{
		base.Start();
		targettingSystemTransform.eulerAngles = new Vector3(90f, 180f, 0f);
		if (targettingSystemRed == null)
		{
			setTargettingSpecifics();
		}
	}

	private void setTargettingSpecifics()
	{
		targettingSystemRed = targettingSystem.transform.Find("red").gameObject;
		targettingSystemGreen = targettingSystem.transform.Find("green").gameObject;
		timeScaler = targettingSystem.transform.Find("chargeRoot/charge");
	}

	protected override IEnumerator beginLock()
	{
		if (targettingSystemRed == null)
		{
			setTargettingSpecifics();
		}
		isLocking = true;
		targettingSystem.SetActive(true);
		targettingSystemRed.SetActive(true);
		targettingSystemGreen.SetActive(false);
		timeScaler.gameObject.SetActive(true);
		for (targettingTimeLeft = targettingTime; targettingTimeLeft > 0f; targettingTimeLeft -= 0.05f)
		{
			timeScaler.localScale = new Vector3(1f, targettingTimeLeft / targettingTime, 1f);
			yield return new WaitForSeconds(0.05f);
		}
		timeScaler.localScale = Vector3.zero;
		targettingSystemRed.SetActive(false);
		targettingSystemGreen.SetActive(true);
		lockedTarget = currentTarget;
		isLocking = false;
	}
}

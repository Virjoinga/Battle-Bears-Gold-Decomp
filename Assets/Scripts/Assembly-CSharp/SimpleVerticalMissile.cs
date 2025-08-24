using System.Collections;
using UnityEngine;

public class SimpleVerticalMissile : Projectile
{
	[SerializeField]
	private float descendSpeed = 4000f;

	[SerializeField]
	private float ascendSpeed = 2000f;

	[SerializeField]
	private float ascendTime = 3f;

	[SerializeField]
	private float pauseAtTopTime = 3f;

	private Rigidbody myRigidbody;

	private void Awake()
	{
		myRigidbody = base.rigidbody;
		base.collider.enabled = false;
	}

	private new void Start()
	{
		if (ServiceManager.Instance != null)
		{
			Item itemByName = ServiceManager.Instance.GetItemByName(configureItemName);
			itemByName.UpdateProperty("descendSpeed", ref descendSpeed, base.EquipmentNames);
			itemByName.UpdateProperty("ascendSpeed", ref ascendSpeed, base.EquipmentNames);
			itemByName.UpdateProperty("ascendTime", ref ascendTime, base.EquipmentNames);
		}
		myRigidbody.velocity = new Vector3(0f, ascendSpeed, 0f);
		StartCoroutine(DelayedStartDescent());
	}

	private IEnumerator DelayedStartDescent()
	{
		yield return new WaitForSeconds(ascendTime);
		base.collider.enabled = true;
		float startLerpTime = Time.fixedTime;
		while (myRigidbody.velocity.y > 0f)
		{
			float currentVelY = Mathf.Lerp(ascendSpeed, 0f, (Time.fixedTime - startLerpTime) / 2f);
			Vector3 currVel = myRigidbody.velocity;
			currVel.y = currentVelY;
			myRigidbody.velocity = currVel;
			yield return null;
		}
		yield return new WaitForSeconds(pauseAtTopTime);
		startLerpTime = Time.fixedTime;
		while (myRigidbody.velocity.y > 0f - descendSpeed)
		{
			float currentVelY2 = Mathf.Lerp(0f, 0f - descendSpeed, (Time.fixedTime - startLerpTime) / 2f);
			Vector3 currVel2 = myRigidbody.velocity;
			currVel2.y = currentVelY2;
			myRigidbody.velocity = currVel2;
			yield return null;
		}
	}

	public void AccountForNetworkDelay(float delay)
	{
		Vector3 position = base.transform.position;
		position.y += delay * ascendSpeed;
		ascendTime -= delay;
		base.transform.position = position;
	}
}

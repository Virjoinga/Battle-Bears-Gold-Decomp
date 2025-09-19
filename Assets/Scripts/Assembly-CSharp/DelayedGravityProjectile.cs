using System.Collections;
using UnityEngine;

public class DelayedGravityProjectile : MonoBehaviour
{
	public float delay = 0.5f;

	public float gravityAmount = 500f;

	protected Rigidbody myRigidbody;

	protected Transform myTransform;

	public bool lookAtDirection = true;

	public int ownerID = -1;

	private void Awake()
	{
		myRigidbody = base.GetComponent<Rigidbody>();
		myTransform = base.transform;
		if (!lookAtDirection)
		{
			base.enabled = false;
		}
	}

	private void OnNetworkDelay(float timeAlreadyElapsed)
	{
		StartCoroutine(delayedGravity(delay - timeAlreadyElapsed));
	}

	private IEnumerator delayedGravity(float actualDelay)
	{
		yield return new WaitForSeconds(actualDelay);
		ConstantForce c = base.gameObject.AddComponent(typeof(ConstantForce)) as ConstantForce;
		Vector3 force = c.force;
		force.y = 0f - gravityAmount;
		c.force = force;
	}

	private void OnCollisionEnter()
	{
		base.enabled = false;
	}

	private void OnTriggerEnter()
	{
		base.enabled = false;
	}

	private void LateUpdate()
	{
		myTransform.LookAt(myTransform.position + myRigidbody.velocity);
	}
}

using System.Collections;
using UnityEngine;

public class ProjectileGravity : Projectile
{
	public float delay = 0.1f;

	public float gravityAmount = 1200f;

	public bool lookAtVelocity = true;

	private Transform _transform;

	private Rigidbody _rigidbody;

	public virtual void Awake()
	{
		_transform = base.transform;
		_rigidbody = base.GetComponent<Rigidbody>();
		StartCoroutine(DelayedGravity());
	}

	private IEnumerator DelayedGravity()
	{
		yield return new WaitForSeconds(delay);
		AddGravity();
	}

	private void AddGravity()
	{
		ConstantForce constantForce = base.gameObject.AddComponent(typeof(ConstantForce)) as ConstantForce;
		Vector3 force = constantForce.force;
		force.y = 0f - gravityAmount;
		constantForce.force = force;
	}

	protected virtual void LateUpdate()
	{
		_transform.LookAt(_transform.position + _rigidbody.velocity);
	}
}

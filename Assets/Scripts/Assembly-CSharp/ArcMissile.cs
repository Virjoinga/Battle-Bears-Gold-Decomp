using System.Collections;
using UnityEngine;

public class ArcMissile : Projectile
{
	[SerializeField]
	private Vector3 _startVelocity;

	[SerializeField]
	private float _descentAcceleration;

	[SerializeField]
	private float _verticalDeceleration;

	[SerializeField]
	private float _colliderWaitTime = 0.5f;

	private Vector3 _velocity;

	private new void Start()
	{
		_velocity = base.transform.root.rotation * _startVelocity;
		base.GetComponent<Rigidbody>().velocity = _velocity;
		base.transform.rotation = Quaternion.LookRotation(_velocity);
		StartCoroutine(DelayedEnableCollider());
	}

	private void FixedUpdate()
	{
		_velocity.y += _verticalDeceleration;
		if (_velocity.y < 0f)
		{
			_velocity += _velocity.normalized * _descentAcceleration;
		}
		base.transform.rotation = Quaternion.LookRotation(_velocity);
		base.GetComponent<Rigidbody>().velocity = _velocity;
	}

	private IEnumerator DelayedEnableCollider()
	{
		base.GetComponent<Collider>().enabled = false;
		yield return new WaitForSeconds(_colliderWaitTime);
		base.GetComponent<Collider>().enabled = true;
	}
}

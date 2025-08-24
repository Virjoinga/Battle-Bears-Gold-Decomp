using UnityEngine;

public class LookAtVelocity : MonoBehaviour
{
	private Rigidbody _rigidBody;

	private Transform _transform;

	private void Awake()
	{
		_rigidBody = base.rigidbody;
		_transform = base.transform;
		if (_transform == null || _rigidBody == null)
		{
			Object.Destroy(this);
		}
	}

	private void LateUpdate()
	{
		_transform.LookAt(base.transform.position + _rigidBody.velocity);
	}
}

using UnityEngine;

public class ForceRigidBodyAwake : MonoBehaviour
{
	private Rigidbody _rigidbody;

	private void Awake()
	{
		_rigidbody = base.GetComponent<Rigidbody>();
	}

	private void Update()
	{
		_rigidbody.WakeUp();
	}
}

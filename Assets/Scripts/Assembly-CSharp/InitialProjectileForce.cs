using UnityEngine;

public class InitialProjectileForce : MonoBehaviour
{
	public Vector3 rotationalForce;

	private void Start()
	{
		base.rigidbody.AddTorque(rotationalForce, ForceMode.VelocityChange);
		Object.Destroy(this);
	}
}

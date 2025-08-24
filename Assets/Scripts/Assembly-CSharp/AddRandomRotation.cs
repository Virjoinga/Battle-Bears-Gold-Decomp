using UnityEngine;

public class AddRandomRotation : MonoBehaviour
{
	public float rotationRange = 100f;

	private Rigidbody myRigidbody;

	private void Start()
	{
		myRigidbody = base.GetComponent<Rigidbody>();
		if (myRigidbody != null)
		{
			myRigidbody.AddTorque(Random.Range(0f - rotationRange, rotationRange), Random.Range(0f - rotationRange, rotationRange), Random.Range(0f - rotationRange, rotationRange), ForceMode.VelocityChange);
		}
		Object.Destroy(this);
	}
}

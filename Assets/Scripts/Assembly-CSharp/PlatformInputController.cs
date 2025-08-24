using UnityEngine;

[AddComponentMenu("Character/Platform Input Controller")]
[RequireComponent(typeof(CharacterMotor))]
public class PlatformInputController : MonoBehaviour
{
	public bool autoRotate = true;

	public float maxRotationSpeed = 360f;

	private CharacterMotor motor;

	public Vector3 directionVector;

	private void Awake()
	{
		motor = GetComponent<CharacterMotor>();
	}

	private void Update()
	{
		if (directionVector != Vector3.zero)
		{
			float magnitude = directionVector.magnitude;
			directionVector /= magnitude;
			magnitude = Mathf.Min(1f, magnitude);
			magnitude *= magnitude;
			directionVector *= magnitude;
		}
		directionVector = Camera.main.transform.rotation * directionVector;
		Quaternion quaternion = Quaternion.FromToRotation(-Camera.main.transform.forward, base.transform.up);
		directionVector = quaternion * directionVector;
		motor.inputMoveDirection = directionVector;
		if (autoRotate && (double)directionVector.sqrMagnitude > 0.01)
		{
			Vector3 v = ConstantSlerp(base.transform.forward, directionVector, maxRotationSpeed * Time.deltaTime);
			v = ProjectOntoPlane(v, base.transform.up);
			base.transform.rotation = Quaternion.LookRotation(v, base.transform.up);
		}
	}

	public Vector3 ProjectOntoPlane(Vector3 v, Vector3 normal)
	{
		return v - Vector3.Project(v, normal);
	}

	public Vector3 ConstantSlerp(Vector3 from, Vector3 to, float angle)
	{
		float t = Mathf.Min(1f, angle / Vector3.Angle(from, to));
		return Vector3.Slerp(from, to, t);
	}
}

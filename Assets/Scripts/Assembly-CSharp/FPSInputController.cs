using UnityEngine;

[RequireComponent(typeof(CharacterMotor))]
[AddComponentMenu("Character/FPS Input Controller")]
public class FPSInputController : MonoBehaviour
{
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
		motor.inputMoveDirection = base.transform.rotation * directionVector;
		motor.inputJump = Input.GetButton("Jump");
	}
}

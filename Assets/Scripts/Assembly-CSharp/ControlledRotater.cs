using UnityEngine;

public class ControlledRotater : MonoBehaviour
{
	[SerializeField]
	private float _turnSpeed;

	private void Update()
	{
		float num = 0f;
		if (Input.GetKey(KeyCode.LeftArrow))
		{
			num += _turnSpeed;
		}
		if (Input.GetKey(KeyCode.RightArrow))
		{
			num -= _turnSpeed;
		}
		if (num != 0f)
		{
			base.transform.localEulerAngles += Vector3.up * num * Time.deltaTime;
		}
	}
}

using SkyVuEngine.Core.Camera;
using UnityEngine;

public class PanningStrategy : ICamera
{
	[SerializeField]
	private float _moveSpeed;

	public float MoveSpeed
	{
		get
		{
			return _moveSpeed;
		}
		set
		{
			_moveSpeed = value;
		}
	}

	protected override void Start()
	{
		base.Start();
		_cameraType = CameraType.Panning;
	}

	protected override void Update()
	{
		float num = 0f;
		float num2 = 0f;
		if (Input.GetKey(KeyCode.W))
		{
			num2 += _moveSpeed;
		}
		if (Input.GetKey(KeyCode.S))
		{
			num2 -= _moveSpeed;
		}
		if (Input.GetKey(KeyCode.A))
		{
			num -= _moveSpeed;
		}
		if (Input.GetKey(KeyCode.D))
		{
			num += _moveSpeed;
		}
		Vector3 position = base.transform.position;
		position.x += num;
		position.z += num2;
		base.transform.position = position;
	}
}

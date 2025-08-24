using SkyVuEngine.Core.Camera;
using UnityEngine;

public class RtsStrategy : ICamera
{
	[SerializeField]
	private float _cameraMoveSpeedMin = 1f;

	[SerializeField]
	private float _cameraMoveSpeedMax = 2.5f;

	[SerializeField]
	private float _cameraRotateSpeed = 0.3f;

	[SerializeField]
	private float _cameraZoomSpeed = 2f;

	[SerializeField]
	private float _farthestZoom = 300f;

	[SerializeField]
	private float _closestZoom = 20f;

	[SerializeField]
	private float _farthestZoomAngle = 60f;

	[SerializeField]
	private float _closestZoomAngle = 30f;

	[SerializeField]
	private GameObject _rotateCenter = null;

	public float CameraMoveSpeedMin
	{
		get
		{
			return _cameraMoveSpeedMin;
		}
		set
		{
			_cameraMoveSpeedMin = ((!(value > _cameraMoveSpeedMax)) ? value : _cameraMoveSpeedMax);
		}
	}

	public float CameraMoveSpeedMax
	{
		get
		{
			return _cameraMoveSpeedMax;
		}
		set
		{
			_cameraMoveSpeedMax = ((!(value < _cameraMoveSpeedMin)) ? value : _cameraMoveSpeedMin);
		}
	}

	public float CameraRotateSpeed
	{
		get
		{
			return _cameraRotateSpeed;
		}
		set
		{
			_cameraRotateSpeed = value;
		}
	}

	public float CameraZoomSpeed
	{
		get
		{
			return _cameraZoomSpeed;
		}
		set
		{
			_cameraZoomSpeed = value;
		}
	}

	public float FarthestZoom
	{
		get
		{
			return _farthestZoom;
		}
		set
		{
			_farthestZoom = ((!(value < _closestZoom)) ? value : _closestZoom);
		}
	}

	public float ClosestZoom
	{
		get
		{
			return _closestZoom;
		}
		set
		{
			_closestZoom = ((!(value > _farthestZoom)) ? value : _farthestZoom);
		}
	}

	public float FarthestZoomAngle
	{
		get
		{
			return _farthestZoomAngle;
		}
		set
		{
			_farthestZoomAngle = ((!(value < _closestZoomAngle)) ? value : _closestZoomAngle);
		}
	}

	public float ClosestZoomAngle
	{
		get
		{
			return _closestZoomAngle;
		}
		set
		{
			_closestZoomAngle = ((!(value > _farthestZoomAngle)) ? value : _farthestZoomAngle);
		}
	}

	public GameObject RotateCenter
	{
		get
		{
			return _rotateCenter;
		}
		set
		{
			_rotateCenter = value;
		}
	}

	protected override void Start()
	{
		base.Start();
		_cameraType = CameraType.TopDownRTS;
	}

	protected override void Update()
	{
		float num = 0f;
		float num2 = 0f;
		float num3 = 0f;
		float num4 = 0f;
		float num5 = (base.transform.position.y - _closestZoom) / (_farthestZoom - _closestZoom);
		float num6 = (_cameraMoveSpeedMax - _cameraMoveSpeedMin) * num5 + _cameraMoveSpeedMin;
		if (Input.GetKey(KeyCode.W))
		{
			num2 += num6;
		}
		if (Input.GetKey(KeyCode.S))
		{
			num2 -= num6;
		}
		if (Input.GetKey(KeyCode.A))
		{
			num -= num6;
		}
		if (Input.GetKey(KeyCode.D))
		{
			num += num6;
		}
		if (Input.GetKey(KeyCode.LeftArrow))
		{
			num3 -= _cameraRotateSpeed;
		}
		if (Input.GetKey(KeyCode.RightArrow))
		{
			num3 += _cameraRotateSpeed;
		}
		if (Input.GetKey(KeyCode.UpArrow))
		{
			num4 -= _cameraZoomSpeed;
		}
		if (Input.GetKey(KeyCode.DownArrow))
		{
			num4 += _cameraZoomSpeed;
		}
		if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
		{
			num *= 2f;
			num2 *= 2f;
			num3 *= 2f;
			num4 *= 2f;
		}
		if (num != 0f || num2 != 0f)
		{
			Vector3 eulerAngles = base.transform.rotation.eulerAngles;
			eulerAngles.x = 0f;
			eulerAngles.y += num3;
			eulerAngles.z = 0f;
			Vector3 vector = new Vector3(num, 0f, num2);
			vector = Quaternion.Euler(eulerAngles) * vector;
			vector.x += base.transform.position.x;
			vector.y += base.transform.position.y;
			vector.z += base.transform.position.z;
			base.transform.position = vector;
		}
		if (num4 != 0f)
		{
			Vector3 position = base.transform.position;
			position.y += num4;
			if (position.y < _closestZoom)
			{
				position.y = _closestZoom;
			}
			if (position.y > _farthestZoom)
			{
				position.y = _farthestZoom;
			}
			float x = (_farthestZoomAngle - _closestZoomAngle) * num5 + _closestZoomAngle;
			Vector3 eulerAngles2 = base.transform.rotation.eulerAngles;
			eulerAngles2.x = x;
			base.transform.rotation = Quaternion.Euler(eulerAngles2);
			base.transform.position = position;
		}
		if (num3 != 0f)
		{
			if (_rotateCenter != null)
			{
				Vector3 worldPosition = new Vector3(_rotateCenter.transform.position.x, 0f, _rotateCenter.transform.position.z);
				float num7 = base.transform.position.x - worldPosition.x;
				float num8 = base.transform.position.z - worldPosition.z;
				float num9 = Mathf.Sqrt(num7 * num7 + num8 * num8);
				float num10 = Mathf.Atan2(num8, num7);
				num10 += num3;
				Vector3 position2 = new Vector3(Mathf.Cos(num10) * num9 + worldPosition.x, base.transform.position.y, Mathf.Sin(num10) * num9 + worldPosition.z);
				base.transform.position = position2;
				base.transform.LookAt(worldPosition);
			}
			else
			{
				Vector3 eulerAngles3 = base.transform.rotation.eulerAngles;
				base.transform.rotation = Quaternion.Euler(eulerAngles3.x, eulerAngles3.y - num3, eulerAngles3.z);
				eulerAngles3.x = 0f;
				eulerAngles3.y += num3;
				eulerAngles3.z = 0f;
				Vector3 vector2 = new Vector3(num3, 0f, 0f);
				vector2 = Quaternion.Euler(eulerAngles3) * vector2;
				vector2.x += base.transform.position.x;
				vector2.y += base.transform.position.y;
				vector2.z += base.transform.position.z;
				base.transform.position = vector2;
			}
		}
	}
}

using UnityEngine;

public class JoulePackMovementController : MonoBehaviour
{
	[SerializeField]
	private float _speed;

	[SerializeField]
	private float _heightToStartDeceleration;

	[SerializeField]
	private float _decelerationAtTop;

	[SerializeField]
	private float _maxFallSpeed;

	[SerializeField]
	private float _maxXSpeed;

	[SerializeField]
	private float _maxZSpeed;

	[SerializeField]
	private float _raycastLength;

	[SerializeField]
	private LayerMask _raycastMask;

	private bool _peaked;

	private float _yStart;

	private float _xSpeed;

	private float _zSpeed;

	private float _horizDecel = 30f;

	private void Start()
	{
		JoulesPack component = base.gameObject.GetComponent<JoulesPack>();
		if (component != null)
		{
			switch (component.Index % 4)
			{
			case 0:
				_xSpeed = _maxXSpeed;
				_zSpeed = 0f;
				break;
			case 1:
				_xSpeed = 0f;
				_zSpeed = _maxZSpeed;
				break;
			case 2:
				_xSpeed = 0f - _maxXSpeed;
				_zSpeed = 0f;
				break;
			case 3:
				_xSpeed = 0f;
				_zSpeed = 0f - _maxZSpeed;
				break;
			default:
				_xSpeed = 0f;
				_zSpeed = 0f;
				break;
			}
		}
		_yStart = base.transform.position.y;
		_peaked = false;
	}

	private void Update()
	{
		if (!(_speed > 0f) && Physics.Raycast(base.transform.position, Vector3.down, _raycastLength, _raycastMask))
		{
			return;
		}
		Vector3 position = base.transform.position;
		position.y += _speed * Time.deltaTime;
		position.x += _xSpeed * Time.deltaTime;
		position.z += _zSpeed * Time.deltaTime;
		if (!_peaked)
		{
			if (base.transform.position.y - _yStart > _heightToStartDeceleration)
			{
				_peaked = true;
			}
		}
		else if (_speed >= 0f - _maxFallSpeed)
		{
			_speed -= _decelerationAtTop * Time.deltaTime;
		}
		if (_xSpeed > 0f)
		{
			_xSpeed = Mathf.Clamp(_xSpeed - _horizDecel * Time.deltaTime, 0f, _xSpeed);
		}
		else if (_xSpeed < 0f)
		{
			_xSpeed = Mathf.Clamp(_xSpeed + _horizDecel * Time.deltaTime, _xSpeed, 0f);
		}
		if (_zSpeed > 0f)
		{
			_zSpeed = Mathf.Clamp(_zSpeed - _horizDecel * Time.deltaTime, 0f, _zSpeed);
		}
		else if (_zSpeed < 0f)
		{
			_zSpeed = Mathf.Clamp(_zSpeed + _horizDecel * Time.deltaTime, _zSpeed, 0f);
		}
		base.transform.position = position;
	}
}
